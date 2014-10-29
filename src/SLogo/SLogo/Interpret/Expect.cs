using System;
using System.Collections.Generic;
using System.Text;
using SLogo.Lex;
using SLogo.Helper;

namespace SLogo.Interpret
{
    /// <summary>
    /// 所有expect方法均不扫描当前字符，
    /// 而是从当前字符开始向后搜寻，搜寻结束时停留在最后一个匹配节点
    /// </summary>
    public partial class Interpreter
    {
        /// <summary>
        /// 从当前链表游标开始向后搜寻指定数目的参数，若不符合要求，则抛出错误。
        /// 同时，计算表达式的值，修改游标为参数的最后一个节点。
        /// 输出的参数列表格式：第一个参数名称为arg0，第二个是arg1...以此类推。
        /// 异常：
        /// NotEnoughArgsException - 参数不足
        /// InvalidExpression - 无效的表达式
        /// </summary>
        /// <param name="iter">标记搜寻结束的游标</param>
        /// <param name="number">参数数量</param>
        private void expectArgs(ref LinkedListNode<Token> iter, out List<Variable> argList, int number)
        {
            argList = new List<Variable>();
            bool hasError = false;

            if (iter.Next == null)
            {
                argList = new List<Variable>();
            }
            if (number < 0)
            {
                throw new ArgumentException("argument number should be >= 0");
            }
            for (int i = 0; i < number; i++)
            {
                iter = iter.Next;

                if (iter == null || iter.Value.Kind == TokenKind.EOF)
                {
                    //未扫描完成便到达链表尾，提示参数不足
                    throw new NotEnoughArgsException();
                }
                else if (iter.Value.Kind != TokenKind.Variables
                    && iter.Value.Kind != TokenKind.Operator
                    && iter.Value.Kind != TokenKind.Float
                    && iter.Value.Kind != TokenKind.Integer
                    && iter.Value.Kind != TokenKind.QuotedString)
                {
                    //扫描到非表达式元素，提示参数不足
                    throw new NotEnoughArgsException();
                }
                else
                {
                    List<Token> expList = new List<Token>();

                    for (; ; iter = iter.Next)
                    {
                        //扫描并计算表达式
                        if (iter.Value.Kind == TokenKind.Float
                            || iter.Value.Kind == TokenKind.Integer
                            || iter.Value.Kind == TokenKind.Variables
                            || iter.Value.Kind == TokenKind.QuotedString)
                        {
                            //给变量命名
                            expList.Add(iter.Value);
                            var next = iter.Next;
                            if (next == null || (next.Value.Kind != TokenKind.Operator)
                                || next.Value.Value == "(")
                            {
                                Variable result = evalExpression(expList, out hasError, "arg" + argList.Count);
                                if (hasError == false)
                                {
                                    //添加变量
                                    argList.Add(result);
                                }
                                else
                                {
                                    isError = true;
                                }
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (iter.Value.Kind == TokenKind.Operator)
                        {
                            //给变量命名
                            expList.Add(iter.Value);

                            if (iter.Value.Value == ")")
                            {
                                var next = iter.Next;
                                if (next == null
                                    || next.Value.Kind == TokenKind.Variables
                                    || next.Value.Kind == TokenKind.QuotedString
                                    || next.Value.Kind == TokenKind.Integer
                                    || (next.Value.Kind == TokenKind.Operator && next.Value.Value == "("))
                                {
                                    Variable result = evalExpression(expList, out hasError, "arg" + argList.Count);
                                    if (hasError == false)
                                    {
                                        //添加变量
                                        argList.Add(result);
                                    }
                                    else
                                    {
                                        isError = true;
                                    }
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Variable result = evalExpression(expList, out hasError, "arg" + argList.Count);
                            if (hasError == false)
                            {
                                //添加变量
                                argList.Add(result);
                            }
                            else
                            {
                                isError = true;
                            }
                            break;
                        }
                    }
                }
            }
            //提示表达式求值异常
            if (hasError)
            {
                isError = true;
                throw new EvalExpressionException();
            }
        }

        /// <summary>
        /// 从当前游标开始向后搜寻第一个[...]形式的指令序列若不符合要求，则抛出错误
        /// 注意：若合法：当前游标应当为方括号。
        /// 同时，修改游标为序列的最后一个节点
        /// 异常：
        /// SeqNotFoundException - 未找到序列
        /// UnclosedSeqException - 序列未闭合
        /// NestedProcException  - 发现子流程嵌套
        /// </summary>
        /// <param name="iter">序列搜寻结束的游标</param>
        /// <returns>抽取的节点序列</returns>
        private LinkedList<Token> expectSeq(ref LinkedListNode<Token> iter)
        {
            int brackets = 0;
            var start = iter = iter.Next;

            if (iter == null || iter.Value.Kind != TokenKind.LeftBracket)
            {
                throw new SeqNotFoundException();
            }
            //else
            //{
            //    brackets++;
            //}

            //开始搜寻
            for (; ; iter = iter.Next)
            {
                if (iter == null || iter.Value.Kind == TokenKind.EOF)
                {
                    throw new UnclosedSeqException();
                }
                else if (iter.Value.Kind == TokenKind.LeftBracket)
                {
                    brackets++;
                }
                else if (iter.Value.Kind == TokenKind.RightBracket)
                {
                    brackets--;
                    //检查是否闭合
                    if (brackets == 0)
                    {
                        //向后移动一格
                        break;
                    }
                }
                //防止子流程嵌套
                else if (String.Compare(iter.Value.Value, "to", true) == 0)
                {
                    throw new NestedProcException();
                }
                else if (String.Compare(iter.Value.Value, "end", true) == 0)
                {
                    throw new NestedProcException();
                }
            }
            //抽取链表并返回
            var seq = current.List.YankList(start, iter);
            //掐头
            seq.RemoveFirst();

            //删掉换行符
            if (seq.Count != 0)
            {
                while (seq.First != null && seq.First.Value.Kind == TokenKind.EOL)
                {
                    seq.RemoveFirst();
                }
                if (seq.Count != 1)
                {
                    for (var node = seq.First; node.Next != null; node = node.Next)
                    {
                        if (node.Next.Value.Kind == TokenKind.EOL)
                        {
                            seq.Remove(node.Next);
                            if (node.Next == null)
                                break;
                        }
                    }
                }
            }

            //返回
            return seq;
        }
    }
}
