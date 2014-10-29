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
        /// 执行if
        /// 异常：
        /// BoolConditionException - 条件不是布尔值
        /// </summary>
        /// <param name="cond">执行条件</param>
        /// <param name="seq">执行序列</param>
        private void ifHandler(Variable cond, LinkedList<Token> seq)
        {
            if (cond.Type == VarType.Double)
            {
                //非布尔表达式异常
                throw new BoolConditionException();
            }
            if (String.Compare(cond.ToString(), "true", true) == 0)
            {
                //表达式为真
                try
                {
                    if (!isError)
                    {
                        lisp(seq);
                    }
                }
                catch (SeriousException)
                {
                    isError = true;
                }
            }
            else if (String.Compare(cond.ToString(), "false", true) != 0)
            {
                //非布尔表达式异常
                throw new BoolConditionException();
            }
        }
    }
}
