using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    internal sealed class DoubleVariable : Variable
    {
        private double val;

        public DoubleVariable(string name, double val)
        {
            this.Name = name;
            this.val = val;
            this.Type = VarType.Double;
        }

        public override double ToDouble()
        {
            return this.val;
        }

        public override string ToString()
        {
            throw new TypeException();
        }
    }
}
