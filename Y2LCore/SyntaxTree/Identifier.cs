using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.SyntaxTree
{
    [Serializable]
    public class Identifier : Expression
    {
    	public string Name{get;private set;}

        public Identifier(string name)
            : base(ExpressionType.Identifier)
        {
            this.Name = name;
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
