using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    public class PrimeExecException: Exception
    {
        public PrimeExecException(string msg)
            : base(msg)
        { }
    }
}
