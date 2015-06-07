using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2LCore.SyntaxTree
{
	[Serializable]
    public sealed class Y2LArray : TypedObject
    {        
        int length;

        object[] values;
        
        public Expression LengthExpression;

        public Y2LArray(PrimitiveType type):base(type,null)
        {            
        }
        public int Length
        {
            set
            {
                if (values == null)
                {
                    length = value;
                    values = new object[length];
                }
            }
            
            get { return length; }

        }
        public object this[int index]
        {
            get { return values[index]; }
            set { values[index]=value;}
        }

    }
}
