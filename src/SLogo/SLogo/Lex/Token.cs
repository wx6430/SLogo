using System;

namespace SLogo.Lex
{
    /// <summary>
    /// ¥ ”Ô÷÷¿‡
    /// </summary>
    public enum TokenKind
    {
        Unknown,
        Identifier,
        Integer,
        Float,
        QuotedString,
        WhiteSpace,
        Keyword,
        EOL,
        EOF,
        Variables,
        Operator,
        LeftBracket,
        RightBracket
    }

    public class Token
    {
        int line;
        int column;
        string value;
        TokenKind kind;

        public Token(TokenKind kind, string value, int line, int column)
        {
            this.kind = kind;
            this.value = value;
            this.line = line;
            this.column = column;
        }

        public int Column
        {
            get { return this.column; }
        }

        public TokenKind Kind
        {
            get { return this.kind; }
        }

        public int Line
        {
            get { return this.line; }
        }

        public string Value
        {
            get { return this.value; }
        }
    }

}
