using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public enum ExpressionType
    {
        BinaryExpression,
        UnaryExpression,
        Identifier,
        Assignment,
        FunctionCall,
        Literal
    }

    public enum BinaryOperator
    {
        Plus = 0,
        Minus,
        Times,
        Divide,

        Equal,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
    }

    public enum UnaryOperator
    {
        Plus,
        Minus
    }

    /// <summary>
    /// Summary description for AstNode.
    /// </summary>
    public class Expression : Statement
    {
        public static string[] BinaryOperatorStrings = { "+", "-", "*", "/","==","<","<=",">",">=" };
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
