using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.Y2LMath
{
    public class FunctionCall : Expression
    {
        string name;
        ExpressionList args;

        public FunctionCall(string name, ExpressionList args)
            : base(ExpressionType.FunctionCall)
        {
            this.name = name;
            this.args = args;
        }

        public ExpressionList Args
        {
            get { return this.args; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder(Name);
            str.Append("(");
            if (args != null)
            {
                foreach (var obj in args)
                {
                    str.Append(obj).Append(",");
                }
                str.Remove(str.Length - 1, 1);
            }
            str.Append(")");
            return str.ToString();
        } 


    }
}
