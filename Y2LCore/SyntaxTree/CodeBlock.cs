using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Y2LCore.SyntaxTree
{
	[Serializable]
	public class CodeBlock : Statement, IEnumerable<Statement>
	{
		public Dictionary<string, TypedObject> _variables;
		public List<Statement> Statements;
		public CodeBlock Parent;
		public string DisplayName;
		
		public CodeBlock():this(String.Empty)
		{}
		public CodeBlock(string displayName)
		{
			DisplayName=displayName;
			_variables = new Dictionary<string, TypedObject>();
			Statements = new List<Statement>();
		}
		
		public virtual PrimitiveType GetPrimitiveType(string name)
		{
			return GetVar(name).PrimitiveType;
		}
		public virtual bool ContainsVar(string name)
		{
			return _variables.ContainsKey(name);
		}
		public void AddVar(string name,PrimitiveType type)
		{
            if(!_variables.ContainsKey(name))
			_variables.Add(name,new TypedObject(type,null));					
		}
		public void AddVar(string name,TypedObject obj)
		{
            if(!_variables.ContainsKey(name))
			_variables.Add(name,obj);					
		}

		public void AssignVar(string name, TypedObject typedObj)
		{
			if(typedObj.Value==null)
				return;
			TypedObject obj = GetVar(name);
			
			if (typedObj.PrimitiveType != obj.PrimitiveType)
			{
				if (typedObj.PrimitiveType != PrimitiveType.Unknown)
					throw new Exception(String.Format("Cannot implicitly convert type '{0}' to '{1}'",
					                                  typedObj.PrimitiveType, obj.PrimitiveType));
			}

			obj.Value = typedObj.Value;
		}
		public TypedObject GetVar(string name)
		{
			return GetVar(name, true);
		}
		public TypedObject GetVar(string name,bool throwException)
		{
			if (_variables.ContainsKey(name))
				return _variables[name];
			else
			{
                
				CodeBlock cb = Parent;
				if (cb != null)
				{
					return cb.GetVar(name);
				}
				if (throwException)
					throw new Exception("The variable '"+name+"' does not exist in the current context");
			}
			return null;
		}
		public override string ToString()
		{
			if(String.IsNullOrEmpty(DisplayName))
			return this.GetType().Name;
			return DisplayName;
		}

		public virtual IEnumerator<Statement> GetEnumerator()
		{
			return Statements.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return null;//Statements.GetEnumerator();
		}
		
	}

	[Serializable]
	public class Module : CodeBlock
	{
		public string Name;
		public List<Function> Functions;

		public Module(string name)
		{
			Functions = new List<Function>();
			this.Name = name;
		}

		public Function GetFunction(string name)
		{
			Function f = new Function(name);
			if (Functions.Contains(f))
				return Functions.Find(a => a.Name == f.Name);
			return null;
		}
		public override IEnumerator<Statement> GetEnumerator()
		{
			foreach (var item in Statements) {
				yield return item;
			}
			foreach (var item in Functions) {
				yield return item;
			}			
		}
		public override string ToString()
		{
			return Name;
		}

	}

	[Serializable]
	public class Function : CodeBlock
	{
		public string Name;
		public PrimitiveType Type;

		public List<FormalParameter> Parameters;

		public Function()
		{ }
		public Function(string name)
		{
			this.Name = name;
		}
		public Function(List<Statement> stmt, List<FormalParameter> parameters, string name, PrimitiveType type)
		{

			this.Statements = stmt;
			this.Parameters = parameters;
			this.Name = name;
			this.Type = type;
		}
		
		public override string ToString()
		{
			StringBuilder str = new StringBuilder(Name);
			str.Append("(");
			if (Parameters != null)
			{
				foreach (var p in Parameters)
					str.Append(p.Type).Append(", ");
				str.Remove(str.Length - 2, 2);
			}
			str.Append(")");

			return str.ToString();

		}
		public override bool Equals(object obj)
		{
			Function f = obj as Function;
			if (f == null)
				return false;
			return (f.Name == this.Name);

		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

    [Serializable]
    public sealed class WhileLoop : CodeBlock
    {
        public string Ident;
        public Expression Condition;

        public override string ToString()
        {
            return "while";
        }
    }

    [Serializable]
    public sealed class ForLoop : CodeBlock
    {
        public string Ident;
        public Statement From;
        public Statement Action;
        public Expression Condition;

        public override string ToString()
        {
            return "for";
        }
    }

    [Serializable]
    public sealed class IfElse : CodeBlock
    {
        public Expression Condition;
        public CodeBlock IfBlock;
        public CodeBlock ElseBlock;
        public List<IfElse> ElseIfStatements;

        public IfElse()
        {
            IfBlock = new CodeBlock("if");
        }
        public override IEnumerator<Statement> GetEnumerator()
        {
            yield return (Statement)IfBlock;

            if (ElseIfStatements != null)
            {
                foreach (var item in ElseIfStatements)
                {
                    yield return item;
                }
            }
            if (ElseBlock != null)
                yield return (Statement)ElseBlock;
        }
    }

}
