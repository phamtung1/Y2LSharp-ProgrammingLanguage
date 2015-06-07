using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.SyntaxTree
{
    [Serializable]
    public class FunctionCall : Expression
    {
        public string Name
        {
            set;
            get;
        }
        List<Expression> args;

        public FunctionCall()
        : base(ExpressionType.FunctionCall)
        {        
        }
        public FunctionCall(string name):this(name,null)
        {
        }
        public FunctionCall(string name, List<Expression> args)
            : base(ExpressionType.FunctionCall)
        {
            this.Name = name;
            this.args = args;
        }

        public List<Expression> Args
        {
            get { return this.args; }
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
    [Serializable]
    public class CSharpFunctionCall : FunctionCall
    {
        public CSharpFunctionCall(string name, List<Expression> args)
            : base(name,args)
        {
        }
    }
}
