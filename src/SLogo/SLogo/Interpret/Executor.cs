using System;
using System.Collections.Generic;
using System.Text;
using SLogo.Lex;
using SLogo.Helper;

namespace SLogo.Interpret
{
    public partial class Interpreter
    {
        /// <summary>
        /// 原语执行器
        /// </summary>
        /// <param name="argList">参数链表，包含命令名</param>
        private void executePrimitive(string name, List<Variable> argList)
        {
            //若已有异常，则只解释不执行
            if (isError)
                return;

            if (argList == null)
            {
                throw new PrimeExecException("needs more arguments");
            }
            primes[name].Execute(argList.ToArray());
        }

        /// <summary>
        /// 子流程执行器，从上下文链中获取变量上下文
        /// 局部变量优先于全局变量
        /// </summary>
        /// <param name="proc">被执行的参数</param>
        private void executeProcedure(Procedure proc)
        {
            //若已有异常，则不执行
            if (isError)
                return;

            try
            {
                lisp(proc.CodeBlock);
                contextChain.Pop();
                if (isError)
                {
                    throw new ProcExecException();
                }
            }
            catch (SeriousException)
            {
                contextChain.Pop();
                throw new ProcExecException();
            }
        }
    }
}
