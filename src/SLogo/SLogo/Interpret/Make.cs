using System;
using System.Collections.Generic;
using System.Text;
using SLogo.Lex;
using SLogo.Helper;
using System.Text.RegularExpressions;

namespace SLogo.Interpret
{
    public partial class Interpreter
    {
        private void makeHandler(Variable[] args)
        {
            var arg0 = args[0];
            var arg1 = args[1];
            if (arg0.Type == VarType.Double)
            {
                throw new PrimeExecException("cannot make number as variable name");
            }

            string varName = arg0.ToString();
            if (!Regex.IsMatch(varName, @"^[a-zA-Z_][a-zA-Z_0-9]*$",
                RegexOptions.Compiled | RegexOptions.Singleline))
            {
                //非法标识符
                throw new PrimeExecException("invalid identifier as variable name");
            }
            //添加全局变量
            if (arg1.Type == VarType.Double)
            {
                globalContext.Add(new DoubleVariable(varName, arg1.ToDouble()));
            }
            else
            {
                globalContext.Add(new StringVariable(varName, arg1.ToString()));
            }
        }
    }
}
