using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public class Identifier : Expression
    {
        string name;

        public Identifier(string name)
            : base(ExpressionType.Identifier)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
