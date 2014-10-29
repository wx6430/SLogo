using System;
using System.Collections.Generic;
using System.Text;
using SLogo.Lex;
using SLogo.Helper;
using System.IO;
using Ciloci.Flee;
using System.Linq;
using System.Text.RegularExpressions;

namespace SLogo.Interpret
{
    public partial class Interpreter
    {
        /// <summary>
        /// 表达式求解
        /// </summary>
        /// <param name="expList">表达式符号序列</param>
        /// <param name="error">错误</param>
        /// <param name="varName">解析出的变量名</param>
        /// <returns>表达式结果变量</returns>
        private Variable evalExpression(IList<Token> expList, out bool hasError, string varName)
        {
            //检查所有变量是否有定义
            bool allDefined = true;
            foreach (var tok in expList)
            {
                if (tok.Kind == TokenKind.Variables
                    && !contextChain.Contains(tok.Value))
                {
                    Stderr.WriteLine("Undefined variable '{0}' @ line {1}, column {2}",
                        tok.Value, tok.Line, tok.Column);
                    allDefined = false;
                }
            }
            if (allDefined == false)
            {
                //未定义的变量存在，提示异常
                hasError = true;
                return null;
            }

            if (expList.Count == 1)
            {
                if (expList[0].Kind == TokenKind.Variables)
                {
                    Variable var = contextChain[expList[0].Value];
                    hasError = false;
                    if (var.Type == VarType.Double)
                    {
                        return new DoubleVariable(varName, var.ToDouble());
                    }
                    else
                    {
                        return new StringVariable(varName, var.ToString());
                    }
                }
                else if (expList[0].Kind == TokenKind.QuotedString)
                {
                    hasError = false;
                    return new StringVariable(varName, expList[0].Value);
                }
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            ExpressionContext context = new ExpressionContext();
            context.Imports.AddType(typeof(System.Math));
            context.Options.CaseSensitive = false;
            context.Options.IntegersAsDoubles = true;

            //拼接表达式
            StringBuilder sb = new StringBuilder();
            foreach (var exp in expList)
            {
                if (exp.Kind == TokenKind.QuotedString)
                {
                    sb.AppendFormat("\"{0}\"", exp.Value);
                }
                else if (exp.Kind == TokenKind.Operator
                    && Lexer.OperatorStrings.Contains(exp.Value.ToLower()))
                {
                    //防止二义性，空格分隔
                    sb.AppendFormat(" {0} ", exp.Value.ToLower());
                }
                else if (exp.Kind == TokenKind.Variables)
                {
                    string mingle = String.Format("_{0}", exp.Value.ToLower());
                    sb.Append(mingle);
                    Variable var = contextChain[exp.Value];
                    if (var.Type == VarType.Double)
                    {
                        context.Variables[mingle] = var.ToDouble();
                    }
                    else
                    {
                        context.Variables[mingle] = var.ToString();
                    }
                }
                else
                {
                    sb.Append(exp.Value.ToLower());
                }
            }

            string expStr = stripExpression(sb.ToString());

            object result;
            try
            {
                IDynamicExpression eDynamic = context.CompileDynamic(expStr);
                result = eDynamic.Evaluate();
            }
            catch (ExpressionCompileException e)
            {
                switch (e.Reason)
                {
                    case CompileExceptionReason.ConstantOverflow:
                        Stderr.WriteLine("Expression constant overflow starts from line {0}, column {1}",
                            expList[0].Line, expList[0].Column);
                        break;
                    case CompileExceptionReason.AmbiguousMatch:
                        Stderr.WriteLine("Ambiguous expression match starts from line {0}, column {1}",
                            expList[0].Line, expList[0].Column);
                        break;
                    case CompileExceptionReason.InvalidFormat:
                        Stderr.WriteLine("Invalid expression format starts from line {0}, column {1}",
                            expList[0].Line, expList[0].Column);
                        break;
                    case CompileExceptionReason.SyntaxError:
                        Stderr.WriteLine("Expression syntax error starts from line {0}, column {1}",
                            expList[0].Line, expList[0].Column);
                        break;
                    case CompileExceptionReason.UndefinedName:
                        Stderr.WriteLine("Undefined variables in expression starts from line {0}, column {1}",
                            expList[0].Line, expList[0].Column);
                        break;
                    default:
                        Stderr.WriteLine("Invalid expression starts from line {0}, column {1}",
                            expList[0].Line, expList[0].Column);
                        break;
                }
                hasError = true;
                return null;
            }

            //如果是布尔表达式
            if (String.Compare(result.ToString(), "true", true) == 0
                || String.Compare(result.ToString(), "false", true) == 0)
            {
                hasError = false;
                return new StringVariable(varName, result.ToString().ToLower());
            }
            try
            {
                double d = Convert.ToDouble(result);
                if (Double.IsNaN(d))
                {
                    //无穷小
                    Stderr.WriteLine("Division by zero in expression starts from line {0}, column {1}",
                        expList[0].Line, expList[0].Column);
                    hasError = true;
                    return null;
                }
                else if (Double.IsInfinity(d))
                {
                    //无穷大
                    Stderr.WriteLine("Division by zero in expression starts from line {0}, column {1}",
                        expList[0].Line, expList[0].Column);
                    hasError = true;
                    return null;
                }
                hasError = false;
                return new DoubleVariable(varName, d);
            }
            catch (Exception)
            {
                //转换错误：非数值表达式
                Stderr.WriteLine("Invalid expression starts from line {0}, column {1}",
                    expList[0].Line, expList[0].Column);
                hasError = true;
                return null;
            }
        }

        /// <summary>
        /// 去除冗余的运算符
        /// </summary>
        /// <param name="str">表达式字符串</param>
        /// <returns>去除冗余运算符后的表达式字符串</returns>
        private string stripExpression(string str)
        {
            while (str.Contains("++") || str.Contains("+-")
                || str.Contains("-+") || str.Contains("--"))
            {
                str = str.Replace("++", "+")
                         .Replace("--", "+")
                         .Replace("-+", "-")
                         .Replace("+-", "-");
            }
            if (str.StartsWith("+"))
            {
                str = str.Remove(0, 1);
            }
            str = str.Replace("=+", "=")
                     .Replace(">+", ">")
                     .Replace("<+", "<")
                     .Replace("*+", "*")
                     .Replace("/+", "/")
                     .Replace("(+", "(")
                     .Replace(",+", ",")
                     .Replace("^+", "^")
                     .Replace("%+", "%");
            return str;
        }
    }
}
