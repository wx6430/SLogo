using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLogo.Lex;

namespace SLogo.Interpret
{
    /// <summary>
    /// 子流程类
    /// </summary>
    internal class Procedure
    {
        /// <summary>
        /// 被执行的代码块
        /// </summary>
        public LinkedList<Token> CodeBlock { get; private set; }
        public string[] Args { get; private set; }
        public string Name { get; private set; }

        public Procedure(string name, string[] args, LinkedList<Token> block)
        {
            this.Name = name;
            this.Args = args;
            this.CodeBlock = block;
        }
    }
}
