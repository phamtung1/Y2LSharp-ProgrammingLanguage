using System.Text;
using System;

namespace Y2LCore.SyntaxTree
{

	[Serializable]
	public abstract class Statement:Expression
	{
		public Expression Expression;
	}
	[Serializable]
	public class FunctionCallStatement : Statement
	{		
		public override string ToString()
		{
			return Expression.ToString();
		}
	}
	[Serializable]
    public class CSharpFunctionCallSatement : FunctionCallStatement
	{
		
		public CSharpFunctionCallSatement()
		{ }
		public override string ToString()
		{
			return Expression.ToString();
		}
	}
	[Serializable]
    public class AssignSatement : Statement
	{
		public string Ident;

		public override string ToString()
		{
			return Ident + "=" + Expression;
		}
	}
    [Serializable]
    public sealed class ArrayItemAssignSatement : AssignSatement
    {        
        public Expression IndexExpression;

        public override string ToString()
        {
            return Ident + "["+IndexExpression+"]=" + Expression;
        }
    }
	[Serializable]
	public class DeclareVariable : AssignSatement
	{
		public PrimitiveType Type;

		public DeclareVariable(PrimitiveType pType)
		{
			Type=pType;
		}
		public override string ToString()
		{
			string s=Type + " " + Ident+"=";
			if(Expression!=null)
				s += Expression.ToString();
			return s;
		}
	}
    [Serializable]
    public sealed class ArrayDeclare : DeclareVariable
    {        
        
        public ArrayDeclare(PrimitiveType pType):base(pType)
        {        
        }
		 
        public override string ToString()
        {
            return Type +"["+Expression+"]";
        }
    }
	[Serializable]
    public sealed class Return : Statement
	{
		public override string ToString()
		{
			return "return " + Expression;
		}
	}

   
	[Serializable]
	public class PrintStatement : Statement
	{
		public override string ToString()
		{
			return "Write(" + Expression + ")";
		}
	}
	[Serializable]
    public sealed class ReadLine : FunctionCall
	{
		
		public override string ToString()
		{
            return "ReadLine()";
		}
	}
	[Serializable]
	public class ReadLineStatement : Statement
	{		
		public override string ToString()
		{
            return "ReadLine()";
		}
	}
	[Serializable]
    public sealed class ReadStmt : CSharpFunctionCallSatement
	{
		public override string ToString()
		{
            return this.GetType().Name;
		}


	}

}