using System.Text;
using Y2LCore.Y2LMath;
using System.Collections.Generic;
using System;
namespace Y2LCore
{
    public enum PrimitiveType
    {
        Boolean,
        Integer,
        Double,
        String,
        Void,
        Unknown,
    }
    public class TypedObject
    {
        public PrimitiveType PrimitiveType;
        public object Value;

        public TypedObject()
        {
        }
        public TypedObject(PrimitiveType pType, object value)
        {
            this.PrimitiveType = pType;
            this.Value = value;
        }
        public override string ToString()
        {
            return this.PrimitiveType.ToString()+" "+Value??Value.ToString();
        }
        public static TypedObject Parse(object value)
        {
            if (value == null)
                return new TypedObject(PrimitiveType.Unknown, null);
            Type type= value.GetType();
            
            if(type ==typeof(bool))
                return new TypedObject(PrimitiveType.Boolean, value);
            if (type == typeof(int))
                    return new TypedObject(PrimitiveType.Integer, value);
            if (type == typeof(double))
                    return new TypedObject(PrimitiveType.Double, value);
            if (type == typeof(string))
                    return new TypedObject(PrimitiveType.String, value);
            
            throw new Exception("Kiểu dữ liệu không hỗ trợ '" + value + "'");
            
        }

    }

    /* <stmt> := var <ident> = <expr>
        | <ident> = <expr>
        | for <ident> = <expr> to <expr> do <stmt> end
        | read_int <ident>
        | print <expr>
        | <stmt> ; <stmt>
      */
    public abstract class Statement
    {
        public Expression Expression;
    }
    public class Block:Statement
    {
        public Dictionary<string,TypedObject>  _variables;

        public Block()
        {
            // use case-insensitive version
            _variables = new Dictionary<string, TypedObject>();
        }
        public virtual bool ContainsVar(string name)
        {
            return _variables.ContainsKey(name);
        }
        public void AddVar(string name, TypedObject value)
        {
            _variables[name] = value;
        }
        public object GetVar(string name)
        {
            if (_variables.ContainsKey(name))
                return _variables[name].Value;
            throw new EvalException("Bien " + name + " chua duoc dinh nghia.", 0, 0);
        }
    }
    public class Module : Block
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
            Function f=new Function(name);
            if (Functions.Contains(f))
                return Functions.Find(a=>a.Name==f.Name);
            return null;
        }
    }
    public class FunctionCallStmt : Statement
    {
        public override string ToString()
        {
            return Expression.ToString();
        }
    }

    // ham <type> <ident> ( [Parameters] ) { Statement }
    public class Function : Block
    {
        public string Name;
        public PrimitiveType Type;
        public List<Statement> Statements;        
        public List<Parameter> Parameters;

        public Function()
        { }
        public Function(string name)
        {
            this.Name = name;
        }
        public Function(List<Statement> stmt, List<Parameter> parameters, string name, PrimitiveType type)
        {
            
            this.Statements=stmt;
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
    // <ident> = <expr>
    public class Assign : Statement
    {
        public string Ident;

        public override string ToString()
        {
            return Ident + "=" + Expression;
        }
    }
    // var <ident> = <expr>
    public class DeclareVar : Assign
    {
        public PrimitiveType PrimitiveType;

        public override string ToString()
        {
            string s=PrimitiveType + " " + Ident+"=";
            if(Expression!=null)
                s += Expression.ToString();
            return s;
        }
    }

    // print <expr>
    public class Print : Statement
    {
        public override string ToString()
        {
            return "Viet(" + Expression + ")";
        }
    }

    public class Return : Statement
    {
        public override string ToString()
        {
            return "TraVe " + Expression;
        }
    }

    // for <ident> = <expr> to <expr> do <stmt> end
    public class ForLoop : Block
    {
        public string Ident;
        public Statement From;
        public Statement Action;
        public Expression Condition;
        public List<Statement> Statements;

        public Block Parent;
    }

    // read_int <ident>
    public class ReadInt : Statement
    {
        public string Ident;
    }

    // <stmt> ; <stmt>
    public class Sequence : Statement
    {
        public Statement First;
        public Statement Second;
    }

    /* <expr> := <string>
     *  | <int>
     *  | <arith_expr>
     *  | <ident>
     */





}