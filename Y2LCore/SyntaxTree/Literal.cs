using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.SyntaxTree
{
    [Serializable]
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
    [Serializable]
    public sealed class StringLiteral : Literal
    {
        public StringLiteral(string value)
            : base(value, PrimitiveType.Chuoi)
        { }
        public override string ToString()
        {
            return "\"" + Value + "\"";
        }
    }

    [Serializable]
    public sealed class IntLiteral : Literal
    {
        public IntLiteral(int value)
            : base(value, PrimitiveType.Nguyen)
        { }
    }
    [Serializable]
    public sealed class DoubleLiteral : Literal
    {
        public DoubleLiteral(double value)
            : base(value, PrimitiveType.Thuc)
        { }
    }
    [Serializable]
    public sealed class BooleanLiteral : Literal
    {
        public BooleanLiteral(bool value)
            : base(value, PrimitiveType.LuanLy)
        { }
    }
}
