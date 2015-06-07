using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore
{
    public enum TokenType
    {
        EOF = 0,  // (EOF)
        Commentline,  // (Comment Line)
        Identifier,  // Identifier
        StringLiteral,
        Number,
        SemiColon, // ;
        Comma,  // ,
        LParen, // (
        RParen, // )

        Function,
        Assign, // =
        Unknown,
        LBracket, // {
        RBracket, // }

        PrimitiveToken,

        Class,
        For,
        Equal, // ==
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,

        Plus,
        Minus,
        Multiply,
        Divide,

        // build-in function
        Write,  // viet
        ReadLine,   // doc

        FunctionCall,
        Return,

        False,
        True,
    }

    public class Token
    {
        public readonly static string[] TokenStrings = new string[]
        {
            "EOF",
            "//",
            "","","",";",",","(",")","ham","=","{","}","","lop","for","<",">","+","-","*","/"
        };
        public TokenType Type;
        public string Text;
        public int Line;
        public int Column;

        public Token(string text, TokenType type,int line,int col):this(text,type)
        {
            this.Line = line;
            this.Column = col;
        }
        public Token() { }
        public Token(string text):this(text,TokenType.Unknown)
        {                        
        }
        public Token(string text, TokenType type)
        {
            this.Type = type;
            this.Text = text;
        }
        
        public override string ToString()
        {
            return Text;
        }
        public static Token Assign
        {
            get { return new Token("=", TokenType.Assign); }
        }
        public static Token SemiColon
        {
            get { return new Token(";", TokenType.SemiColon); }
        }
        public static Token Comma
        {
            get { return new Token(",", TokenType.Comma); }
        }
        public static Token LParen{
            get { return new Token("(", TokenType.LParen); }
        }
        public static Token RParen
        {
            get { return new Token(")", TokenType.RParen); }
        }
        public static Token LBracket
        {
            get { return new Token("{", TokenType.LBracket); }
        }
        public static Token RBracket
        {
            get { return new Token("}", TokenType.RBracket); }
        }

        public static Token Module {
            get { return new Token("Lop", TokenType.Class); }
        }
    }
    public class PrimitiveToken : Token
    {
        public PrimitiveType PrimitiveType;

        public PrimitiveToken() { }
        public PrimitiveToken(string text, PrimitiveType primitiveType)
            : base(text,TokenType.PrimitiveToken)
        {
            
            this.PrimitiveType = primitiveType;
        }
        public override string ToString()
        {
            return PrimitiveType.ToString();
        }
    }

}
