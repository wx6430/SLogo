using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Lex
{
    /// <summary>
    /// 词语源，为解释器提供词语序列
    /// </summary>
    public interface ITokenFeed : IEnumerable<Token>
    {
        new IEnumerator<Token> GetEnumerator();
    }
}
