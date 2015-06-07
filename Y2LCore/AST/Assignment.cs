using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public class Assignment : Expression
    {
        Expression variable;
        Expression rhs;

        public Assignment(Expression variable, Expression rhs)
            : base(ExpressionType.Assignment)
        {
            this.variable = variable;
            this.rhs = rhs;
        }

        public Expression Rhs
        {
            get { return this.rhs; }
        }

        public Expression Variable
        {
            get { return this.variable; }
        }
        public override string ToString()
        {
            return variable + "=" + rhs;
        }
    }
}
