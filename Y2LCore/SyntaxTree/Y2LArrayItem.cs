using System;

namespace Y2LCore.SyntaxTree
{
	[Serializable]
    public sealed class Y2LArrayItem : Identifier
	{
		public Expression IndexExpression;
		
		public Y2LArrayItem(string name):base(name)
		{
			
		}
		public override string ToString()
		{
			return Name+"["+IndexExpression+"]";
		}
	}
}
