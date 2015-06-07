using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Y2LCore.Y2LMath
{
    public class Evaluator
    {
        /// <summary>
        /// stores variables name -> object mapping
        /// </summary>
        Module _module;

        public Evaluator(Module module)
        {
            _module = module;
        }
        public void Evaluate()
        {
            Function func= _module.GetFunction("Main");            
            Evaluate(func);
        }

        public object Evaluate(Function function)
        {
            //if (function.Parameters != null)
            //{
            //    foreach (var stmt in function.Parameters)
            //    {
            //        EvaluateStatement(function, stmt);
            //    }
            //}

            foreach (var stmt in function.Statements)
            {
                if (stmt is Return)
                    return EvaluateStatement(function, stmt);
                else
               EvaluateStatement(function, stmt);                          
            }
            return null;
        }
        public object EvaluateStatement(Block function, Statement stmt)
        {
            if (stmt is DeclareVar)
                function.AddVar(((DeclareVar)stmt).Ident, TypedObject.Parse(EvalExpression(function, stmt.Expression)));
            else if (stmt is Assign)
                function.AddVar(((Assign)stmt).Ident, TypedObject.Parse(EvalExpression(function, stmt.Expression)));
            else if (stmt is Return)
            {
                return EvalExpression(function, stmt.Expression);
            }
            else if (stmt is StringLiteral)
                return ((StringLiteral)stmt).Value;
            else if (stmt is ForLoop)
            {
                ForLoop forloop = (ForLoop)stmt;
                foreach (var sm in forloop.Statements)
                {
                    if (sm is Return)
                        return EvaluateStatement(forloop, sm);
                    else
                        EvaluateStatement(forloop, sm);
                }
            }
            else if (stmt is FunctionCallStmt)
            {
                EvalFunctionCall(function, (FunctionCall)stmt.Expression);
            }
            return null;
        }


        public object EvalExpression(Block function,Expression exp)
        {
            if (exp == null)
                return null;
            switch (exp.ExpType)
            {
                case ExpressionType.Assignment:
                    return EvalAssignment(function, (Assignment)exp);

                case ExpressionType.BinaryExpression:
                    return EvalBinaryExpression(function,(BinaryExpression)exp);

                case ExpressionType.Identifier:
                    return EvalIdentifier(function,(Identifier)exp);

                case ExpressionType.Literal:
                    return EvalLiteral((Literal)exp);

                case ExpressionType.FunctionCall:
                    return EvalFunctionCall(function, (FunctionCall)exp);

                case ExpressionType.UnaryExpression:
                    return EvalUnaryExpression(function,(UnaryExpression)exp);

                default:
                    throw new Exception("Invalid expression type: " + exp.ExpType.ToString());
            }

        }

        /// <summary>
        /// performs assignment and returns LHS of assignment
        /// </summary>
        object EvalAssignment(Block function, Assignment exp)
        {
            if (exp.Variable.ExpType != ExpressionType.Identifier)
                throw new Exception("LHS of assignment must be variable.");

            string name = ((Identifier)exp.Variable).Name;

            object val = EvalExpression(function,exp.Rhs);

            if (function.ContainsVar(name))
                function.AddVar(name, TypedObject.Parse(val));
            else
                _module.AddVar(name, TypedObject.Parse(val));

            return val;
        }

        object EvalUnaryExpression(Block function, UnaryExpression exp)
        {
            object val = EvalExpression(function,exp.Exp);

            switch (exp.Op)
            {
                case UnaryOperator.Plus:
                    return +(double)val;
                case UnaryOperator.Minus:
                    return -(double)val;
                default:
                    throw new Exception("Invalid Unary Operator: " + exp.Op.ToString());

            }
        }

        object EvalBinaryExpression(Block function, BinaryExpression exp)
        {
            object lhs = EvalExpression(function,exp.Lhs);
            object rhs = EvalExpression(function, exp.Rhs);

            switch (exp.Operator)
            {
                case BinaryOperator.Plus:
                    return Add(lhs, rhs);

                case BinaryOperator.Minus:
                    return (double)lhs - (double)rhs;

                case BinaryOperator.Times:
                    return (double)lhs * (double)rhs;

                case BinaryOperator.Divide:
                    return (double)lhs / (double)rhs;
                case BinaryOperator.Equal:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                    return Compare(exp.Operator,lhs, rhs);

                default:
                    throw new Exception("Toan tu khong hop le: " + exp.Operator.ToString());
            }
        }
        object Add(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();

            if (lt == typeof(string) || rt == typeof(string))
                return lhs.ToString() + rhs.ToString();
            else if (lt == typeof(bool) || rt == typeof(bool))
                throw new Exception("Khong the cong kieu Boolean");
            else if (lt == typeof(double) || rt == typeof(double))
                return double.Parse(lhs.ToString()) + double.Parse(rhs.ToString());
            else// if (lt == typeof(int) && rt == typeof(int))
                return (int)lhs + (int)rhs;

        }
        bool Compare(BinaryOperator op, object lhs, object rhs)
        {
            IComparable lo= (IComparable)lhs;
            IComparable ro = (IComparable)rhs;

            if (op == BinaryOperator.Equal)
                return lo.CompareTo(ro) == 0;
            else if (op == BinaryOperator.GreaterThan)
                return lo.CompareTo(ro) > 0;
            else if (op == BinaryOperator.GreaterThanOrEqual)
                return lo.CompareTo(ro) >= 0;
            else if (op == BinaryOperator.LessThan)
                return lo.CompareTo(ro) < 0;
            else if (op == BinaryOperator.LessThanOrEqual)
                return lo.CompareTo(ro) <= 0;
            else
                return false;
        }

        object EvalIdentifier(Block block, Identifier exp)
        {
            object value=null;
            
            if (block.ContainsVar(exp.Name))
                value = (object)block.GetVar(exp.Name);
            else if(block is ForLoop)
            {
                Block parent=((ForLoop)block).Parent;
                if(parent.ContainsVar(exp.Name))
                    value = (object)parent.GetVar(exp.Name);
            }
            if (value == null)    
                value = (object)_module.GetVar(exp.Name);

            return value;
        }

        object EvalLiteral(Literal exp)
        {
            return exp.Value;
        }

        object EvalFunctionCall(Block function, FunctionCall exp)
        {
            string fname = exp.Name.ToLower();
            switch (fname)
            {
                case "sin":
                    {
                        AssertArgCount(exp, 1);
                        object val1 = EvalExpression(function,exp.Args[0]);
                        return Math.Sin((double)val1);
                    }
                case "cos":
                    {
                        AssertArgCount(exp, 1);
                        object val1 = EvalExpression(function,exp.Args[0]);
                        return Math.Cos((double)val1);
                    }
                case "abs":
                    {
                        AssertArgCount(exp, 1);
                        object val1 = EvalExpression(function, exp.Args[0]);
                        return Math.Abs((double)val1);
                    }
                case "pow":
                    {
                        AssertArgCount(exp, 2);
                        object val1 = EvalExpression(function, exp.Args[0]);
                        object val2 = EvalExpression(function, exp.Args[1]);
                        return Math.Pow((double)val1, (double)val2);

                    }
                case "sqrt":
                    {
                        AssertArgCount(exp, 1);
                        object val1 = EvalExpression(function, exp.Args[0]);
                        return Math.Sqrt((double)val1);
                    }

                case "exp":
                    {
                        AssertArgCount(exp, 1);
                        object val1 = EvalExpression(function, exp.Args[0]);
                        return Math.Exp((double)val1);
                    }
                case "log":
                    {
                        AssertArgCount(exp, 1);
                        object val1 = EvalExpression(function, exp.Args[0]);
                        return Math.Log((double)val1);
                    }
                default:
                    if (_module.Functions.Contains(new Function(exp.Name)))
                    {                        

                        Function f = _module.GetFunction(exp.Name);

                        for (int i = 0; i < exp.Args.Count; i++)
                        {
                                                        
                            f.AddVar(f.Parameters[i].Name,new TypedObject(f.Type,EvalExpression(function, exp.Args[i])));
                        }

                        return Evaluate(f);
                    }
                    throw new Exception("Khong tim thay ham '" + exp.Name+"'");
            }
        }

        /// <summary>
        /// this method is called by function call.
        /// If exp argument count is not count, then it throws EvalException
        /// </summary>
        void AssertArgCount(FunctionCall exp, int count)
        {
            if (exp.Args.Count != count)
            {
                throw new Exception("Function " + exp.Name + " is expecting " + count + " parameters.");
            }
        }



    }
}
