using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    internal sealed class StringVariable: Variable
    {
        private string val;

        public StringVariable(string name, string val)
        {
            this.Name = name;
            this.val = val;
            this.Type = VarType.String;
        }

        public override double ToDouble()
        {
            throw new TypeException();
        }
        public override string ToString()
        {
            return this.val;
        }
    }
}
