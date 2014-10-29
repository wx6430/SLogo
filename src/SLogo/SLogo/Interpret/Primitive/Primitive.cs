using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    /// <summary>
    /// 原语，即内置命令
    /// </summary>
    internal class Primitive
    {
        private PrimitiveHandler handler;
        public int ArgCount { get; private set; }
        public string Name {get; private set;}

        public Primitive(string name, int argCount, PrimitiveHandler handler)
        {
            this.Name = name;
            this.ArgCount = argCount;
            this.handler = handler;
        }

        public void Execute(Variable[] args)
        {
            this.handler(args);
        }
    }
}
