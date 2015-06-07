using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Y2LCore;

namespace Y2LCore.SyntaxTree
{
	[Serializable]
	public struct FormalParameter
	{
		public string Name;

		public PrimitiveType Type;
		
		public bool IsArray;
		
		public FormalParameter(string name, PrimitiveType type)
		{
			Name = name;
			Type = type;
			IsArray=false;
		}
		public override string ToString()
		{
			return Type + " " + Name;
		}
	}

}
