using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Y2LCore
{
    public sealed class Scanner
    {
        const char EOF = '\0';

        private IList<Token> result;
        int _line = 1, _col = 0;
        char ch;
        int _index = -1;
        string _buffer;
        int _tokenIndex = -1;

        public Scanner(TextReader input)
        {
            this.result = new List<Token>();
            this._buffer = input.ReadToEnd();
            input.Dispose();
            this.Scan();
        }
        public Token Peek()
        {
            if (_tokenIndex >= result.Count - 1)
                return new Token("EOF", TokenType.EOF);
            return result[_tokenIndex + 1];
        }
        public Token Next()
        {
            if (_tokenIndex >= result.Count - 1)
                return new Token("EOF", TokenType.EOF, result[_tokenIndex].Line, result[_tokenIndex].Column);
            _tokenIndex++;
            return result[_tokenIndex];
        }
        private char PeekChar()
        {
            if (_index >= _buffer.Length)
            {
                return EOF;
            }
            return _buffer[_index + 1];
        }
        private void NextChar()
        {
            if (_index >= _buffer.Length - 1)
            {
                ch = EOF;
                return;
            }
            _index++;
            ch = _buffer[_index];

            if ((ch == '\n'))
            {
                _line++;
                _col = 0;
            }
            else
                _col++;

        }

        private void Scan()
        {
            NextChar();
            while (true)
            {
                // Scan individual tokens
                while (char.IsWhiteSpace(ch))
                {
                    NextChar();
                }
                if (ch == EOF)
                    break;
                if (char.IsLetter(ch) || ch == '_')
                {
                    // keyword or identifier
                    this.result.Add(KeywordAndIdent());
                }
                else if (ch == '"')
                {
                    // string literal
                    Token token = StringLiteral();
                    this.result.Add(token);
                }
                else if (char.IsDigit(ch))
                {
                    // numeric literal
                    Token token = NumbericLiteral();
                    this.result.Add(token);
                }
                else
                {
                    Terminal();
                }
            }
        }

        private void Terminal()
        {

            switch (ch)
            {
                case '+':
                    NextChar();
                    this.result.Add(new Token("+", TokenType.Plus, _line, _col));
                    break;

                case '-':
                    NextChar();
                    this.result.Add(new Token("-", TokenType.Minus, _line, _col));
                    break;

                case '*':
                    NextChar();
                    this.result.Add(new Token("*", TokenType.Multiply, _line, _col));
                    break;

                case '/':
                    NextChar();
                    this.result.Add(new Token("/", TokenType.Divide, _line, _col));
                    break;

                case '=':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        this.result.Add(new Token("==", TokenType.Equal, _line, _col));
                    }
                    else
                        this.result.Add(new Token("=", TokenType.Assign, _line, _col));
                    break;

                case ';':
                    NextChar();
                    this.result.Add(new Token(";", TokenType.SemiColon, _line, _col));
                    break;
                case '(':
                    NextChar();
                    this.result.Add(new Token("(", TokenType.LParen, _line, _col));
                    break;
                case ')':
                    NextChar();
                    this.result.Add(new Token(")", TokenType.RParen, _line, _col));
                    break;
                case '{':
                    NextChar();
                    this.result.Add(new Token("{", TokenType.LBracket, _line, _col));
                    break;
                case '}':
                    NextChar();
                    this.result.Add(new Token("}", TokenType.RBracket, _line, _col));
                    break;
                case ',':
                    NextChar();
                    this.result.Add(new Token(",", TokenType.Comma, _line, _col));
                    break;
                case '<':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        this.result.Add(new Token("<=", TokenType.LessThanOrEqual, _line, _col));
                    }
                    else
                        this.result.Add(new Token("<", TokenType.LessThan, _line, _col));
                    break;

                case '>':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        this.result.Add(new Token(">=", TokenType.GreaterThanOrEqual, _line, _col));
                    }
                    else
                        this.result.Add(new Token(">", TokenType.GreaterThan, _line, _col));
                    break;
                default:
                    throw new System.Exception("Scanner encountered unrecognized character '" + ch + "'");
            }
        }

        private Token NumbericLiteral()
        {

            StringBuilder str = new StringBuilder();
            int line = _line;
            int col = _col;
            while (char.IsDigit(ch) || ch == '.')
            {
                str.Append(ch);
                NextChar();

                if (ch == EOF)
                {
                    break;
                }
            }
            Token token = new Token(str.ToString(), TokenType.Number, line, col);
            return token;
        }

        private Token StringLiteral()
        {
            StringBuilder str = new StringBuilder();
            int line = _line;
            int col = _col;
            NextChar(); // skip the '"'

            if (ch == EOF)
            {
                throw new System.Exception("unterminated string literal");
            }

            while (ch != '"')
            {
                str.Append(ch);
                NextChar();

                if (ch == EOF)
                {
                    throw new System.Exception("unterminated string literal");
                }
            }

            // skip the terminating "
            NextChar();
            Token token = new Token(str.ToString(), TokenType.StringLiteral);
            token.Line = line;
            token.Column = col;
            return token;
        }

        private Token KeywordAndIdent()
        {

            StringBuilder strB = new StringBuilder();
            int line = _line;
            int col = _col;
            while (char.IsLetter(ch) || ch == '_' || char.IsDigit(ch))
            {
                strB.Append(ch);
                NextChar();

                if (ch == EOF)
                {
                    break;
                }
            }
            string str = strB.ToString();
            Token token;
            switch (str.ToUpper())
            {
                case "DUNG":
                    token = new Token(str, TokenType.True);
                    break;
                case "SAI":
                    token = new Token(str, TokenType.False);
                    break;
                case "TRAVE": // return
                    token = new Token(str, TokenType.Return);
                    break;
                case "VIET": // print
                    token = new Token(str, TokenType.Write);
                    break;
                case "LOP": // class
                    token = new Token(str, TokenType.Class);
                    break;
                case "HAM": // function
                    token = new Token(str, TokenType.Function);
                    break;
                case "LAP": // for
                    token = new Token(str, TokenType.For);
                    break;
                case "INT":
                    token = new PrimitiveToken(str, PrimitiveType.Integer);
                    break;
                case "DOUBLE":
                    token = new PrimitiveToken(str, PrimitiveType.Double);
                    break;
                case "BOOL":
                    token = new PrimitiveToken(str, PrimitiveType.Boolean);
                    break;
                case "STRING":
                    token = new PrimitiveToken(str, PrimitiveType.String);
                    break;
                case "VOID":
                    token = new PrimitiveToken(str, PrimitiveType.Void);
                    break;

                default:
                    token = new Token(str, TokenType.Identifier);
                    break;
            }

            token.Line = line;
            token.Column = col;
            return token;
        }

    }
}