using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Y2LCore.Y2LMath;

namespace Y2LCore
{
    public class Parameter
    {
        public string Name;

        public PrimitiveType Type;

        public object Expr;

        public Parameter(string name, PrimitiveType type)
            : this(name, type, null)
        { }
        public Parameter(string name, PrimitiveType type, object expr)
        {
            Name = name;
            Type = type;
            Expr = expr;
        }
        public override string ToString()
        {
            return Type + " " + Name + "=" + Expr??Expr.ToString();
        }
    }

}
