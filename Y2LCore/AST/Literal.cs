using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public class Literal : Expression
    {
        public PrimitiveType PrimitiveType;
        public object Value;
        public Literal(object value,PrimitiveType type)
            : base(ExpressionType.Literal)
        {
            this.Value = value;

            this.PrimitiveType = type;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
    // <string> := " <string_elem>* "
    public class StringLiteral : Literal
    {
        public StringLiteral(string value)
            : base(value, PrimitiveType.String)
        { }
    }

    //// <int> := <digit>+
    public class IntLiteral : Literal
    {
        public IntLiteral(int value)
            : base(value, PrimitiveType.Integer)
        { }
    }

    public class DoubleLiteral : Literal
    {
        public DoubleLiteral(double value)
            : base(value, PrimitiveType.Double)
        { }
    }
    public class BooleanLiteral : Literal
    {
        public BooleanLiteral(bool value)
            : base(value, PrimitiveType.Boolean)
        { }
    }
}
