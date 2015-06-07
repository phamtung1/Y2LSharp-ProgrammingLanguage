using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.SyntaxTree
{
    public enum ExpressionType
    {
        BinaryExpression,
        UnaryExpression,
        Identifier,
        Assignment,
        FunctionCall,
        Literal,
        Csharp,        
    }

    public enum BinaryOperator
    {
        Add,
        Substract,
        Multiply,
        Divide,
        Modulo,

        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        
        And,
        Or,
    }

    public enum UnaryOperator
    {
        Plus,
        Minus
    }
    [Serializable]
    public class Expression
    {
        public static string[] BinaryOperatorStrings = {
        "+", "-", "*", "/", "%", "==","!=", "<", "<=", ">", ">=","&&","||" };
        ExpressionType expType;
        public Expression() { }
        public Expression(ExpressionType expType)
        {
            this.expType = expType;
        }

        public ExpressionType ExpType
        {
            get { return this.expType; }
        }

    }
}

