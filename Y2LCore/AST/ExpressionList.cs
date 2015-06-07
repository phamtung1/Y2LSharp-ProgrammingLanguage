using System;
using System.Collections.Generic;

namespace Y2LCore.Y2LMath
{
    public class ExpressionList :List<Expression>
    {
        public ExpressionList()
        {
        }

        public new Expression this[int index]
        {
            get
            {
                return (Expression)base[index];
            }
            set
            {
                base[index] = value;
            }
        }



    }
}
