using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SLogo.Lex
{
    /// <summary>
    /// ����δ�պϴ���
    /// </summary>
    public class BadQuoteException : Exception
    {
        public Token QuotedString { get; private set; }

        public BadQuoteException(Token token)
            : base()
        {
            this.QuotedString = token;
        }
    }



    /// <summary>
    /// ���ַ������зִ�
    /// </summary>
    public class Lexer : ITokenFeed
    {
        const char EOF = (char)0;

        int line;
        int column;
        int pos;	// �������е�λ��

        string data;

        bool ignoreWhiteSpace;
        private static readonly string operatorChars = "+-*/()<=>,^%";
        private static readonly string[] operatorStrings = 
            { "and", "or", "not", "xor" , "sin", "cos", "tan", "cot",
            "abs", "round", "sqrt", "pow", "floor", "ceiling", "exp",
            "log", "log10"};

        int saveLine;
        int saveCol;
        int savePos;

        public Lexer(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            data = reader.ReadToEnd();

            reset();
        }

        public Lexer(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;

            reset();
        }

        /// <summary>
        /// �ʷ�������֧�ֵ������ַ�
        /// </summary>
        public static char[] OperatorChars
        {
            get { return operatorChars.ToCharArray(); }
        }

        public static string[] OperatorStrings
        {
            get { return operatorStrings; }
        }

        /// <summary>
        /// ���Ϊ�棬�����Կո񣬵���Ȼ�����EOL
        /// </summary>
        public bool IgnoreWhiteSpace
        {
            get { return this.ignoreWhiteSpace; }
            set { this.ignoreWhiteSpace = value; }
        }

        private void reset()
        {
            this.ignoreWhiteSpace = true;

            line = 1;
            column = 1;
            pos = 0;
        }

        protected char la(int count)
        {
            if (pos + count >= data.Length)
                return EOF;
            else
                return data[pos + count];
        }

        protected char consume()
        {
            char ret = data[pos];
            pos++;
            column++;

            return ret;
        }

        protected Token createToken(TokenKind kind, string value)
        {
            return new Token(kind, value, line, column);
        }

        protected Token createToken(TokenKind kind)
        {
            string tokenData = data.Substring(savePos, pos - savePos);
            return new Token(kind, tokenData, saveLine, saveCol);
        }

        private Token next()
        {
        ReadToken:

            char ch = la(0);

            if (ch == EOF)
            {
                return createToken(TokenKind.EOF, string.Empty);
            }
            else if (ch == ' ' || ch == '\t')
            {
                if (this.ignoreWhiteSpace)
                {
                    consume();
                    goto ReadToken;
                }
                else
                    return readWhitespace();
            }
            else if (ch == '\r')
            {
                startRead();
                consume();
                if (la(0) == '\n')
                    consume();	// DOS/Windows���У�\r\n

                line++;
                column = 1;

                return createToken(TokenKind.EOL);
            }
            else if (ch == '\n')
            {
                startRead();
                consume();
                line++;
                column = 1;

                return createToken(TokenKind.EOL);
            }
            else if (ch == ';')
            {
                //ע��
                eatComment();
                goto ReadToken;
            }
            else if (ch == '.')
            {
                if (Char.IsNumber(la(1)))
                {
                    return readNumber();
                }
                else
                {
                    return createToken(TokenKind.Unknown);
                }
            }
            else if (ch == '[')
            {
                startRead();
                consume();
                return createToken(TokenKind.LeftBracket);
            }
            else if (ch == ']')
            {
                startRead();
                consume();
                return createToken(TokenKind.RightBracket);
            }
            else if (Char.IsNumber(ch))
            {
                return readNumber();
            }
            else if (ch == ':')
            {
                //TODO
                if (la(1) == EOF)
                {
                    startRead();
                    consume();
                    return createToken(TokenKind.Unknown);
                }
                else if (Char.IsLetter(la(1)) || la(1) == '_')
                {
                    return readVariable();
                }
                else
                {
                    startRead();
                    consume();
                    return createToken(TokenKind.Unknown);
                }
            }
            else if (isOperatorChar(ch))
            {
                startRead();
                consume();
                return createToken(TokenKind.Operator);
            }
            else if (ch == '"')
            {
                return readString();
            }
            else
            {
                if (Char.IsLetter(ch) || ch == '_')
                {
                    return readIdentifier();
                }
                else
                {
                    startRead();
                    consume();
                    return createToken(TokenKind.Unknown);
                }
            }
        }

        /// <summary>
        /// ��¼��ʼ��ȡ��λ��
        /// </summary>
        private void startRead()
        {
            saveLine = line;
            saveCol = column;
            savePos = pos;
        }

        protected void eatComment()
        {
            startRead();

            consume();

            while (true)
            {
                char ch = la(0);
                if (ch != '\n' && ch != EOF)
                    consume();
                else
                {
                    //consume();
                    //line++;
                    //column = 1;
                    break;
                }
            }
        }

        /// <summary>
        /// ��ȡ���пհ׷������������з���
        /// </summary>
        /// <returns></returns>
        protected Token readWhitespace()
        {
            startRead();

            consume(); //�Ե�ǰ��Ŀհ׷�

            while (true)
            {
                char ch = la(0);
                if (ch == '\t' || ch == ' ')
                    consume();
                else
                    break;
            }

            return createToken(TokenKind.WhiteSpace);

        }

        /// <summary>
        /// �������֣�DIGIT+ ("." DIGIT*)?
        /// </summary>
        /// <returns></returns>
        protected Token readNumber()
        {
            startRead();

            bool hadDot;
            if (la(0) == '.')
            {
                hadDot = true;
            }
            else
            {
                hadDot = false;
            }

            consume();  // �����һ������

            while (true)
            {
                char ch = la(0);
                if (Char.IsDigit(ch))
                    consume();
                else if (ch == '.' && !hadDot)
                {
                    hadDot = true;
                    consume();
                }
                else
                    break;
            }

            if (hadDot)
            {
                return createToken(TokenKind.Float);
            }
            else
            {
                return createToken(TokenKind.Integer);
            }
        }

        /// <summary>
        /// ����һ����ʶ��
        /// </summary>
        protected Token readIdentifier()
        {
            startRead();

            consume();  // �����һ����ĸ

            while (true)
            {
                char ch = la(0);
                if (Char.IsLetterOrDigit(ch) || ch == '_')
                    consume();
                else
                    break;
            }

            Token tok = createToken(TokenKind.Identifier);
            if (operatorStrings.Contains(tok.Value.ToLower()))
            {
                return createToken(TokenKind.Operator, tok.Value);
            }
            return tok;
        }

        protected Token readVariable()
        {
            consume(); // �����һ����ĸ
            startRead();

            while (true)
            {
                char ch = la(0);
                if (Char.IsLetterOrDigit(ch) || ch == '_')
                    consume();
                else
                    break;
            }

            return createToken(TokenKind.Variables);
        }

        /// <summary>
        /// ��ȡ˫���Ű�Χ�������ַ���
        /// </summary>
        /// <returns>�������ַ����Ĵ�</returns>
        protected Token readString()
        {
            startRead();
            consume();
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char ch = la(0);
                if (ch == EOF)
                {
                    throw new BadQuoteException(createToken(TokenKind.QuotedString));
                }
                else if (ch == '\r')	// ����CR
                {
                    sb.Append(ch);
                    consume();
                    if (la(0) == '\n')	// DOS & windows
                    {
                        Token tok = createToken(TokenKind.QuotedString);
                        consume();
                        line++;
                        column = 1;
                        throw new BadQuoteException(tok);
                    }
                }
                else if (ch == '\n')	// �����е�����
                {
                    Token tok = createToken(TokenKind.QuotedString);
                    consume();
                    line++;
                    column = 1;
                    throw new BadQuoteException(tok);
                }
                else if (ch == '\\')
                {
                    consume();
                    if (la(0) == '"' || la(0) == '\\')
                    {
                        sb.Append(la(0));
                        consume();
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                else if (ch == '"')
                {
                    consume();
                    break;
                }
                else
                {
                    sb.Append(ch);
                    consume();
                }
            }

            return createToken(TokenKind.QuotedString, sb.ToString());
        }

        /// <summary>
        /// �ַ��Ƿ�Ϊ�����
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected bool isOperatorChar(char c)
        {
            return (operatorChars.IndexOf(c) != -1);
        }

        #region ITokenProvider ��Ա

        public IEnumerator<Token> GetEnumerator()
        {
            try
            {
                Token tok;
                do
                {
                    tok = this.next();
                    yield return tok;

                } while (tok.Kind != TokenKind.EOF);
            }
            finally
            {
                this.reset();
            }
        }

        #endregion

        #region IEnumerable ��Ա

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
