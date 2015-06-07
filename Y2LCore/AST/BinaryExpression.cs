using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public class BinaryExpression : Expression
    {
        Expression lhs;
        Expression rhs;
        BinaryOperator op;

        public BinaryExpression(Expression lhs, BinaryOperator op, Expression rhs)
            : base(ExpressionType.BinaryExpression)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public Expression Lhs
        {
            get { return this.lhs; }
        }

        public BinaryOperator Operator
        {
            get { return this.op; }
        }

        public Expression Rhs
        {
            get { return this.rhs; }
        }

        public override string ToString()
        {
            return  "(" + lhs + Expression.BinaryOperatorStrings[(int)op] + rhs + ")";
        }

    }
    public class ComparisionExpression : BinaryExpression
    {
        public ComparisionExpression(Expression lhs, BinaryOperator op, Expression rhs)
            : base(lhs, op, rhs)
        {
        }
    }
}
