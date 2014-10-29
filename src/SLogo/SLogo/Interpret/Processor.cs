using System;
using System.Collections.Generic;
using System.Text;
using SLogo.Lex;
using SLogo.Helper;
using System.Linq;

namespace SLogo.Interpret
{
    public partial class Interpreter
    {
        /// <summary>
        /// 纪念John McCarthy，优雅的Lisp及其七个公理的创造者
        /// </summary>
        /// <param name="list">链表</param>
        private void lisp(LinkedList<Token> list)
        {
            var original = current;
            for (current = list.First; current != null && current.Value.Kind != TokenKind.EOF; current = current.Next)
            {
                Token tok = current.Value;

                //换行符，无视
                if (tok.Kind == TokenKind.EOL)
                {
                    continue;
                }
                //非法字符
                else if (tok.Kind == TokenKind.Unknown)
                {
                    Stderr.WriteLine("Unexpected character {0} '{1}' @ line {2}, column {3}",
                        tok.Kind.ToString(), tok.Value, tok.Line, tok.Column);
                }
                //标识符
                else if (tok.Kind == TokenKind.Identifier)
                {
                    //关键字是特殊的标识符
                    identifierProcess();
                }
                else
                {
                    //设置错误位
                    isError = true;
                    Stderr.WriteLine("Unexpeceted {0} '{1}' @ line {2}, column {3}",
                        tok.Kind.ToString(), tok.Value, tok.Line, tok.Column);
                }
                if (current == null)
                {
                    break;
                }
            }
            //恢复当前指令
            current = original;
        }

        #region 处理标识符

        /// <summary>
        /// 处理标识符
        /// </summary>
        /// <param name="tok">词语</param>
        private void identifierProcess()
        {
            Token tok = current.Value;

            //repeat
            if (String.Compare(tok.Value, "repeat", true) == 0)
            {
                repeatProcess(tok);
            }
            else if (String.Compare(tok.Value, "if", true) == 0)
            {
                ifProcess(tok);
            }
            else if (String.Compare(tok.Value, "ifelse", true) == 0)
            {
                ifElseProcess(tok);
            }
            else if (String.Compare(tok.Value, "to", true) == 0)
            {
                toProcess(tok);
            }
            else if (String.Compare(tok.Value, "end", true) == 0)
            {
                //未闭合的end
                isError = true;
                Stderr.WriteLine("Unexpeceted '{1}' @ line {2}, column {3}",
                    tok.Kind.ToString(), tok.Value, tok.Line, tok.Column);
            }
            //处理原语
            else if (primes.Contains(tok.Value))
            {
                primeProcess(tok);
            }
            else
            {
                if (procs.Contains(tok.Value))
                {
                    procProcess(tok);
                }
                else
                {
                    //未定义的标识符
                    Stderr.WriteLine("Undefined procedure or primitive '{1}' @ line {2}, column {3}",
                        tok.Kind.ToString(), tok.Value, tok.Line, tok.Column);
                }
            }
        }

        #endregion

        #region 处理原语

        /// <summary>
        /// 处理原语
        /// </summary>
        /// <param name="tok">原语词语</param>
        private void primeProcess(Token tok)
        {
            var prime = primes[tok.Value];
            var iter = current;
            try
            {
                List<Variable> argList;
                //检查参数
                expectArgs(ref iter, out argList, prime.ArgCount);
                //执行
                executePrimitive(tok.Value, argList);
            }
            catch (NotEnoughArgsException)
            {
                //参数不足
                isError = true;
                Stderr.WriteLine("{0}: need more arguments @ line {1}, column {2}",
                    tok.Value.ToUpper(), tok.Line, tok.Column);
            }
            catch (EvalExpressionException)
            {
                //表达式求值异常
                isError = true;
            }
            catch (PrimeExecException e)
            {
                //原语执行异常
                isError = true;
                Stderr.WriteLine("{0}: {1} @ line {2}, column {3}",
                    tok.Value.ToUpper(), e.Message, tok.Line, tok.Column);
            }
            finally
            {
                //光标移动
                current = iter;
            }
        }

        #endregion

        #region 处理Repeat

        /// <summary>
        /// 处理Repeat
        /// </summary>
        /// <param name="tok">repeat所在词语</param>
        private void repeatProcess(Token tok)
        {
            var iter = current;

            //需要一个参数
            try
            {
                List<Variable> argList;
                //检查参数
                expectArgs(ref iter, out argList, 1);
                var seq = expectSeq(ref iter);
                //检查参数并执行
                repeatHandler(argList[0], seq);
            }
            catch (NotEnoughArgsException)
            {
                //参数不足
                isError = true;
                Stderr.WriteLine("No 'REPEAT' times specified @ line {0}, column {1}",
                    tok.Line, tok.Column);
                throw new SeriousException();
            }
            catch (EvalExpressionException)
            {
                //表达式求值异常
                isError = true;
                throw new SeriousException();
            }
            catch (SeqNotFoundException)
            {
                //没找到序列
                isError = true;
                Stderr.WriteLine("'REPEAT' expects a statement @ line {0}, column {1}",
                    tok.Line, tok.Column);
            }
            catch (UnclosedSeqException)
            {
                //未闭合的序列
                isError = true;
                Stderr.WriteLine("Unclosed '[' of 'REPEAT' statement @line {0}, column {1}",
                    iter.Value.Line, iter.Value.Column);
            }
            catch (NestedProcException)
            {
                //子流程嵌套
                isError = true;
                Stderr.WriteLine("'{0}' cannot be defined within statement @ line {1}, column {2}",
                    iter.Value.Value.ToUpper(), iter.Value.Column);
                throw new SeriousException();
            }
            catch (RepeatTimesException)
            {
                //重复次数非正整数
                isError = true;
                Stderr.WriteLine("'REPEAT' expects an positive interger repeat times @ line {0}, column {1}",
                    tok.Line, tok.Column);
            }
            finally
            {
                //光标移动
                current = iter;
            }
        }

        #endregion

        #region 处理If

        /// <summary>
        /// 处理If
        /// </summary>
        /// <param name="tok">if所在词语</param>
        private void ifProcess(Token tok)
        {
            var iter = current;

            //需要一个参数
            try
            {
                List<Variable> argList;
                //检查参数
                expectArgs(ref iter, out argList, 1);
                var seq = expectSeq(ref iter);
                //检查参数并执行
                ifHandler(argList[0], seq);
            }
            catch (NotEnoughArgsException)
            {
                //参数不足
                isError = true;
                Stderr.WriteLine("No '{0}' condition specified @ line {1}, column {2}",
                    tok.Value.ToUpper(), tok.Line, tok.Column);
                throw new SeriousException();
            }
            catch (EvalExpressionException)
            {
                //表达式求值异常
                isError = true;
                throw new SeriousException();
            }
            catch (SeqNotFoundException)
            {
                //没找到序列
                isError = true;
                Stderr.WriteLine("'IF' expects a statement @ line {0}, column {1}",
                    tok.Line, tok.Column);
            }
            catch (UnclosedSeqException)
            {
                //未闭合的序列
                isError = true;
                Stderr.WriteLine("Unclosed '[' of 'IF' statement @line {0}, column {1}",
                    iter.Value.Line, iter.Value.Column);
            }
            catch (NestedProcException)
            {
                //子流程嵌套
                isError = true;
                Stderr.WriteLine("'{0}' cannot be defined within statement @ line {1}, column {2}",
                    iter.Value.Value.ToUpper(), iter.Value.Column);
                throw new SeriousException();
            }
            catch (BoolConditionException)
            {
                //非条件表达式
                isError = true;
                Stderr.WriteLine("'IF' expects a bool expression @ line {0}, column {1}",
                    tok.Line, tok.Column);
            }
            finally
            {
                //光标移动
                current = iter;
            }
        }

        #endregion

        #region 处理IfElse

        /// <summary>
        /// 处理IfElse
        /// </summary>
        /// <param name="tok">ifelse所在词语</param>
        private void ifElseProcess(Token tok)
        {
            var iter = current;

            //需要一个参数
            try
            {
                List<Variable> argList;
                //检查参数
                expectArgs(ref iter, out argList, 1);
                var ifSeq = expectSeq(ref iter);
                var elseSeq = expectSeq(ref iter);
                //检查参数并执行
                ifElseHandler(argList[0], ifSeq, elseSeq);
            }
            catch (NotEnoughArgsException)
            {
                //参数不足
                isError = true;
                Stderr.WriteLine("No 'IFELSE' condition specified @ line {0}, column {1}",
                    tok.Value.ToUpper(), tok.Line, tok.Column);
                throw new SeriousException();
            }
            catch (EvalExpressionException)
            {
                //表达式求值异常
                isError = true;
                throw new SeriousException();
            }
            catch (SeqNotFoundException)
            {
                //没找到if序列
                isError = true;
                Stderr.WriteLine("Not enough statements for 'IFELSE' @ line {0}, column {1}",
                    tok.Line, tok.Column);
            }
            catch (UnclosedSeqException)
            {
                //未闭合的序列
                isError = true;
                Stderr.WriteLine("Unclosed '[' of 'IFELSE' statement @line {0}, column {1}",
                    iter.Value.Line, iter.Value.Column);
            }
            catch (NestedProcException)
            {
                //子流程嵌套
                isError = true;
                Stderr.WriteLine("'{0}' cannot be defined within statement @ line {1}, column {2}",
                    iter.Value.Value.ToUpper(), iter.Value.Column);
                throw new SeriousException();
            }
            catch (BoolConditionException)
            {
                //非条件表达式
                isError = true;
                Stderr.WriteLine("'IFELSE' expects a bool expression @ line {0}, column {1}",
                    tok.Line, tok.Column);
            }
            finally
            {
                //光标移动
                current = iter;
            }
        }

        #endregion

        #region 处理To

        /// <summary>
        /// 处理To
        /// </summary>
        /// <param name="tok">to所在词语</param>
        private void toProcess(Token tok)
        {
            var iter = current;
            var end = iter;
            bool hasNested = false;
            int nest = 0;
            List<string> argList = new List<string>();

            for (; end != null && end.Value.Kind != TokenKind.EOF; end = end.Next)
            {
                if (end != iter && end.Value.Value.ToLower() == "to")
                {
                    if (hasNested == false)
                    {
                        //嵌套
                        Stderr.WriteLine("Syntax error: nested 'TO' statement @ line {0}, column {1}",
                            end.Value.Line, end.Value.Column);
                    }
                    hasNested = true;
                    nest++;
                }
                if (end.Value.Value.ToLower() == "end")
                {
                    if (nest != 0)
                    {
                        nest--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (end == null || end.Value.Kind == TokenKind.EOF)
            {
                //未闭合的To语句
                isError = true;
                Stderr.WriteLine("Syntax error: incomplete 'TO' statement @ line {0}, column {1}",
                    iter.Value.Line, iter.Value.Column);
                current = end;
                return;
            }
            if (hasNested)
            {
                //有嵌套
                isError = true;
                current = end;
                return;
            }
            if (iter.Previous != null && iter.Previous.Value.Kind != TokenKind.EOL)
            {
                //没换行
                isError = true;
                Stderr.WriteLine("Syntax error: 'TO' statement should start from a new line @ line {0}, column {1}",
                    iter.Value.Line, iter.Value.Column);
                current = end;
                return;
            }
            if (iter.Next == end
                || iter.Next.Value.Kind != TokenKind.Identifier
                || keywords.Contains(iter.Next.Value.Value.ToLower()))
            {
                //没函数名
                isError = true;
                Stderr.WriteLine("Syntax error: invalid procedure name '{0}' @ line {1}, column {2}",
                    iter.Next.Value.Value, iter.Next.Value.Line, iter.Next.Value.Column);
                current = end;
                return;
            }
            if (primes.Contains(iter.Next.Value.Value))
            {
                //原语不能覆盖
                isError = true;
                Stderr.WriteLine("Cannot overwrite primitive '{0}' @ line {1}, column {2}",
                    iter.Next.Value.Value, iter.Next.Value.Line, iter.Next.Value.Column);
                current = end;
                return;
            }
            if (procs.Contains(iter.Next.Value.Value))
            {
                //子流程不能覆盖
                isError = true;
                Stderr.WriteLine("Cannot overwrite procedure '{0}' @ line {1}, column {2}",
                    iter.Next.Value.Value, iter.Next.Value.Line, iter.Next.Value.Column);
                current = end;
                return;
            }

            string procName = iter.Next.Value.Value;
            iter = iter.Next.Next;

            try
            {
                fetchProcArgs(argList, ref iter, end);
                var block = current.List.YankList(iter, end);
                procs.Add(new Procedure(procName, argList.ToArray(), block));
            }
            catch (InvalidProcException)
            {
                isError = true;
            }
            current = end;
            return;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="argList">参数列表</param>
        /// <param name="iter">开始游标</param>
        /// <param name="end">END游标</param>
        private void fetchProcArgs(List<string> argList, ref LinkedListNode<Token> iter, LinkedListNode<Token> end)
        {
            while (iter.Value.Kind == TokenKind.Variables)
            {
                //添加参数，检查重复参数
                if (argList.Contains(iter.Value.Value.ToLower()))
                {
                    Stderr.WriteLine("Syntax error: duplicated argument in 'TO' statement @ line {0}, column {1}",
                        iter.Value.Line, iter.Value.Column);
                    throw new InvalidProcException();
                }
                argList.Add(iter.Value.Value.ToLower());
                iter = iter.Next;
            }
            if (iter == end || end.Previous.Value.Kind != TokenKind.EOL)
            {
                //没有函数体，END前无换行
                Stderr.WriteLine("Syntax error: 'END' should start from a new line @ line {0}, column {1}",
                    end.Value.Line, end.Value.Column);
                throw new InvalidProcException();
            }
            else if (iter.Value.Kind != TokenKind.EOL)
            {
                //参数后无换行
                Stderr.WriteLine("Syntax error: unexpected {0} '{1}' @ line {2}, column {3}",
                    iter.Value.Kind.ToString(), iter.Value.Value, iter.Value.Line, iter.Value.Column);
                throw new InvalidProcException();
            }
            if (end.Next != null && end.Next.Value.Kind != TokenKind.EOL && end.Next.Value.Kind != TokenKind.EOF)
            {
                //END不独占一行
                Stderr.WriteLine("Syntax error: unexpected {0} '{1}' @ line {2}, column {3}",
                    end.Next.Value.Kind.ToString(), end.Next.Value.Value, end.Next.Value.Line, end.Next.Value.Column);
            }
        }

        #endregion

        #region 处理子流程

        /// <summary>
        /// 处理子流程
        /// </summary>
        /// <param name="proc">子流程</param>
        private void procProcess(Token tok)
        {
            var proc = procs[tok.Value];
            var iter = current;
            try
            {
                List<Variable> argList;
                //检查参数
                expectArgs(ref iter, out argList, proc.Args.Length);
                //准备上下文
                Context ctx = new Context();
                for (int i = 0; i < proc.Args.Length; i++)
                {
                    argList[i].Name = proc.Args[i];
                    ctx.Add(argList[i]);
                }
                contextChain.Push(ctx);
                //执行
                executeProcedure(proc);
                //executePrimitive(tok.Value, argList);
            }
            catch (NotEnoughArgsException)
            {
                //参数不足
                isError = true;
                Stderr.WriteLine("{0}: need more arguments @ line {1}, column {2}",
                    tok.Value.ToUpper(), tok.Line, tok.Column);
            }
            catch (EvalExpressionException)
            {
                //表达式求值异常
                isError = true;
            }
            catch (ProcExecException)
            {
                Stderr.WriteLine("--> In most recent call: '{0}' @ line {1}, column {2}",
                    tok.Value, tok.Line, tok.Column);
                //子流程执行异常
                isError = true;
                //恢复上下文
                contextChain.Pop();
            }
            catch (ProcStopSignal)
            {
                //捕获STOP
                contextChain.Pop();
            }
            finally
            {
                //光标移动
                current = iter;
            }
        }

        #endregion
    }
}
