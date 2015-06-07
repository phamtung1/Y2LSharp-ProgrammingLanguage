using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public class UnaryExpression : Expression
    {
        Expression exp;
        UnaryOperator op;

        public UnaryExpression(UnaryOperator op, Expression exp)
            : base(ExpressionType.UnaryExpression)
        {
            this.exp = exp;
            this.op = op;
        }

        public Expression Exp
        {
            get { return this.exp; }
        }

        public UnaryOperator Op
        {
            get { return this.op; }
        }

    }
}
