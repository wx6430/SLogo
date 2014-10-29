using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SLogo.Interpret
{
    /// <summary>
    /// 变量只有两种类型：浮点数和字符串
    /// 变量值类型是隐式的
    /// </summary>
    public abstract class Variable
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (Regex.IsMatch(value, @"^[a-zA-Z_][a-zA-Z_0-9]*$", RegexOptions.Compiled | RegexOptions.Singleline))
                {
                    this.name = value.ToLower();
                }
                else
                {
                    throw new IllegalVarNameException();
                }
            }
        }
        public VarType Type { get; protected set; }

        //类型转换，会抛出异常
        public abstract double ToDouble();
        public abstract override string ToString();
    }
}
