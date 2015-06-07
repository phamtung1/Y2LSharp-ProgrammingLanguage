using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Y2LCore.SyntaxTree
{
    public enum TokenType
    {
        EOF = 0,  // (EOF)
        LineComment,  // (Comment Line)
        Identifier,  // Identifier
        StringLiteral,
        Number,
        SemiColon = 5, // ;
        Comma,  // ,
        LParen, // (
        RParen, // )

        Function,
        Assign=10, // =
        Unknown,
        LBracket, // {
        RBracket, // }
        LSquareBracket, //[
        RSquareBracket, //]
        PrimitiveToken,

        Class,
        For,
        While,

        Equal=20, // ==
        NotEqual, // !=
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        
        And,
        Or,

        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo, //%
        // build-in function
        Write,
        ReadLine, 

        FunctionCall,
        Return,

        False,
        True,

        CsharpIns, // Csharp Instruction

        If,
        Else,
        ElseIf,

    }
    [DebuggerStepThrough]
    public class Token
    {
        public TokenType Type;
        public string Text;
        public int Line;
        public int Column;
        
        public Token(){}
        public Token(string text, TokenType type,int line,int col):this(text,type)
        {
            this.Line = line;
            this.Column = col;
        }
        
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
            get { return new Token("module", TokenType.Class); }
        }
    }
    public sealed class PrimitiveToken : Token
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
