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
        /// 执行repeat
        /// 异常：
        /// RepeatTimesException - 重复次数不是整数
        /// </summary>
        /// <param name="times">执行次数</param>
        /// <param name="seq">执行序列</param>
        private void repeatHandler(Variable times, LinkedList<Token> seq)
        {
            try
            {
                double d = times.ToDouble();
                if (Math.Round(d) - d > 0.01)
                {
                    throw new TypeException();
                }
            }
            catch (TypeException)
            {
                throw new RepeatTimesException();
            }
            int rt = (int)Math.Round(times.ToDouble());
            if (rt <= 0)
            {
                throw new RepeatTimesException();
            }
            //必须是正整数

            try
            {
                for (int i = 0; i < rt; i++)
                {
                    if (!isError)
                    {
                        lisp(seq);
                    }
                }
            }
            catch (SeriousException)
            {
                isError = true;
            }
        }
    }
}
