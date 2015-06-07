using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Y2LCore.SyntaxTree;

namespace Y2LCore
{
	public sealed class Scanner
	{
		const char EOF = '\0';

		private IList<Token> result;
		int _line = 1, _col = 0;
		char _ch;
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
				_ch = EOF;
				return;
			}
			_index++;
			_ch = _buffer[_index];

			if ((_ch == '\n'))
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
				while (char.IsWhiteSpace(_ch))
				{
					NextChar();
				}
				if (_ch == EOF)
					break;
				if (char.IsLetter(_ch) || _ch == '_')
				{
					this.result.Add(KeywordAndIdent());
				}
				else if (_ch == '"')
				{
					Token token = StringLiteral();
					this.result.Add(token);
				}
				else if (char.IsDigit(_ch))
				{
					Token token = NumbericLiteral();
					this.result.Add(token);
				}

				else if (_ch == '#') // C# Instruction
				{
					NextChar();
					
					if (_ch != '[')
						throw new Exception("[ expected");
					Token token = CSharpInstruction();
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

			switch (_ch)
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
					if(_ch=='/')
					{
						NextChar();
						this.result.Add(ReadCommentLine());
					}else
						this.result.Add(new Token("/", TokenType.Divide, _line, _col));
					break;
				case '%':
					NextChar();
					this.result.Add(new Token("%", TokenType.Modulo, _line, _col));
					break;
				case '!':
					NextChar();
					if (_ch == '=')
					{
						NextChar();
						this.result.Add(new Token("!=", TokenType.NotEqual, _line, _col));
					}
					else
						goto Error;
					break;
				case '=':
					NextChar();
					if (_ch == '=')
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
					if (_ch == '=')
					{
						NextChar();
						this.result.Add(new Token("<=", TokenType.LessThanOrEqual, _line, _col));
					}
					else
						this.result.Add(new Token("<", TokenType.LessThan, _line, _col));
					break;

				case '>':
					NextChar();
					if (_ch == '=')
					{
						NextChar();
						this.result.Add(new Token(">=", TokenType.GreaterThanOrEqual, _line, _col));
					}
					else
						this.result.Add(new Token(">", TokenType.GreaterThan, _line, _col));
					break;
				case '&':
					NextChar();
					if (_ch == '&')
					{
						NextChar();
						this.result.Add(new Token("&&", TokenType.And, _line, _col));
					}
					else
						goto Error;
					break;
				case '|':
					NextChar();
					if (_ch == '|')
					{
						NextChar();
						this.result.Add(new Token("||", TokenType.Or, _line, _col));
					}
					else
						goto Error;
					break;
				case '[':
					NextChar();
					this.result.Add(new Token("[", TokenType.LSquareBracket  , _line, _col));
					break;
				case ']':
					NextChar();
					this.result.Add(new Token("]", TokenType.RSquareBracket, _line, _col));
					break;
				default:
				Error:
					throw new System.Exception("Unrecognized character '" + _ch + "'");
			}
		}
		private Token ReadCommentLine(){
			StringBuilder s=new StringBuilder("//");
			
			while(_ch!='\r' && _ch!='\n' && _ch!=EOF)
			{
				s.Append(_ch);
				NextChar();
			}
			
			return new Token(s.ToString(), TokenType.LineComment, _line, 0);
		}
		// e.g [System.Console.WriteLine]
		private Token CSharpInstruction()
		{
			
			StringBuilder str = new StringBuilder();
			int line = _line;
			int col = _col;
			NextChar(); // read [
			while (_ch != ']')
			{
				str.Append(_ch);
				NextChar();

				if (_ch == EOF)
				{
					break;
				}
			}
			NextChar();
			Token token = new Token(str.ToString(), TokenType.CsharpIns, line, col);
			return token;
		}
		private Token NumbericLiteral()
		{

			StringBuilder str = new StringBuilder();
			int line = _line;
			int col = _col;
			while (char.IsDigit(_ch) || _ch == '.')
			{
				str.Append(_ch);
				NextChar();

				if (_ch == EOF)
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
			NextChar();

			if (_ch == EOF)
			{
				throw new System.Exception("Unterminate string literal");
			}

			while (_ch != '"')
			{
				str.Append(_ch);
				NextChar();

				if (_ch == EOF)
				{
                    throw new System.Exception("Unterminate string literal");
				}
			}

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
			while (char.IsLetter(_ch) || _ch == '_' || char.IsDigit(_ch))
			{
				strB.Append(_ch);
				NextChar();

				if (_ch == EOF)
				{
					break;
				}
			}
			string str = strB.ToString();
			Token token;
			switch (str)
			{
				case "if":
					token = new Token(str, TokenType.If);
					break;
				case "else":
					token = new Token(str, TokenType.Else);
					break;
				case "elseif":
					token = new Token(str, TokenType.ElseIf);
					break;
				case "true":
					token = new Token(str, TokenType.True);
					break;
				case "false":
					token = new Token(str, TokenType.False);
					break;
				case "return": // return
					token = new Token(str, TokenType.Return);
					break;
				case "Write": // write
					token = new Token(str, TokenType.Write);
					break;
				case "ReadLine":
					token = new Token(str, TokenType.ReadLine);
					break;
				case "module": 
					token = new Token(str, TokenType.Class);
					break;
				case "function": // function
					token = new Token(str, TokenType.Function);
					break;
				case "for": // for
					token = new Token(str, TokenType.For);
					break;
				case "while": // while
					token = new Token(str, TokenType.While);
					break;
				case "int":
					token = new PrimitiveToken(str, PrimitiveType.Nguyen);
					break;
				case "double":
					token = new PrimitiveToken(str, PrimitiveType.Thuc);
					break;
				case "luanly":
					token = new PrimitiveToken(str, PrimitiveType.LuanLy);
					break;
				case "string":
					token = new PrimitiveToken(str, PrimitiveType.Chuoi);
					break;
				case "void":
					token = new PrimitiveToken(str, PrimitiveType.Khong);
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