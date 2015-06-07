using System;
using System.Text;
using Y2LCore;
using Y2LCore.SyntaxTree;


namespace Y2LInterpreter
{
    public class Y2LExecutor
    {
        Module _module;

        public Y2LExecutor(Module module)
        {
            _module = module;
        }
        public void Execute()
        {
            foreach (var stmt in _module.Functions)
            {
                ExeStatement(_module, stmt);
            }
            Function func = _module.GetFunction("Main");
            Execute(func);
        }

        public object Execute(Function function)
        {
            object ret = null;
            foreach (var stmt in function.Statements)
            {
                if (stmt is Return)
                    return ExeStatement(function, stmt);
                else
                    ret = ExeStatement(function, stmt);
            }
            return ret;
        }
        public object ExeStatement(CodeBlock block, Statement stmt)
        {

            if (stmt is PrintStatement)
            {
                PrintStatement p = (PrintStatement)stmt;
                string s = ExeExpression(block, p.Expression).ToString();
                s = s.Replace(@"\n", "\n");
                s = s.Replace(@"\t", "\t");

                Console.Write(s);

            }
            else if (stmt is ReadLineStatement)
            {
                Console.ReadLine();
            }
            else if (stmt is DeclareVariable)
            {
                if (stmt is ArrayDeclare)
                {
                    ArrayDeclare dec = (ArrayDeclare)stmt;
                    Y2LArray array = (Y2LArray)block.GetVar(dec.Ident);
                    array.Length = (int)ExeExpression(block, array.LengthExpression);
                }
                else
                {
                    DeclareVariable dec = (DeclareVariable)stmt;
                    block.AddVar(dec.Ident, dec.Type);
                    block.AssignVar(dec.Ident, TypedObject.Parse(ExeExpression(block, stmt.Expression)));
                }
            }
            else if (stmt is AssignSatement)
            {
                if (stmt is ArrayItemAssignSatement)
                {
                    ArrayItemAssignSatement assign = (ArrayItemAssignSatement)stmt;
                    Y2LArray array = (Y2LArray)block.GetVar(assign.Ident);
                    object value = ExeExpression(block, assign.Expression);
                    int index = (int)ExeExpression(block, assign.IndexExpression);
                    array[index] = value;
                }
                else
                {
                    //AssignSatement assign = (AssignSatement)stmt;
                    //block.AssignVar(assign.Ident,TypedObject.Parse(EvalExpression(block, stmt.Expression)));

                    AssignSatement assign = (AssignSatement)stmt;
                    if (assign.Expression is CSharpFunctionCall)
                    {
                        block.AssignVar(assign.Ident, TypedObject.Parse(
                            ExeFunctionCall(block, (FunctionCall)stmt.Expression)));
                    }

                    else
                    {
                        TypedObject obj = TypedObject.Parse(ExeExpression(block, stmt.Expression));
                        block.AssignVar(assign.Ident, obj);
                    }
                }
            }
            else if (stmt is Return)
            {
                return ExeExpression(block, stmt.Expression);
            }
            else if (stmt is ForLoop)
            {
                ForLoop forLoop = (ForLoop)stmt;

                ExeStatement(forLoop, forLoop.From);
                while (true)
                {
                    bool b = (bool)ExeExpression(forLoop, forLoop.Condition);
                    if (b)
                    {
                        foreach (Statement sm in forLoop.Statements)
                        {
                            if (sm is Return)
                                return ExeStatement(forLoop, sm);
                            else
                                ExeStatement(forLoop, sm);
                        }
                    }
                    else
                        break;

                    ExeStatement(forLoop, forLoop.Action);
                }

            }
            else if (stmt is IfElse)
            {
                bool b = false;
                IfElse ifelse = (IfElse)stmt;
                object ret = null;
                if ((bool)ExeExpression(ifelse.IfBlock, ifelse.Condition))
                {
                    foreach (Statement st in ifelse.IfBlock.Statements)
                    {
                        if (st is Return)
                            return ExeStatement(ifelse.IfBlock, st);
                        else
                            ret = ExeStatement(ifelse.IfBlock, st);
                    }
                    if (ret == null)
                        return true;
                    return ret;
                }
                else if (ifelse.ElseIfStatements != null)
                {
                    foreach (IfElse elseif in ifelse.ElseIfStatements)
                    {
                        if ((bool)ExeExpression(elseif.IfBlock, elseif.Condition))
                        {
                            ret = ExeStatement(elseif.IfBlock, elseif);
                            break;
                        }
                    }
                    if (ret != null)
                        return ret;
                }

                if (!b && ifelse.ElseBlock != null)
                {
                    foreach (Statement st in ifelse.ElseBlock.Statements)
                    {
                        if (st is Return)
                            return ExeStatement(ifelse.ElseBlock, st);
                        else
                            ret = ExeStatement(ifelse.ElseBlock, st);
                    }
                }
                return ret;
            }
            else if (stmt is WhileLoop)
            {
                WhileLoop whileLoop = (WhileLoop)stmt;

                while (true)
                {
                    bool b = (bool)ExeExpression(whileLoop, whileLoop.Condition);
                    if (b)
                    {
                        foreach (Statement sm in whileLoop.Statements)
                        {
                            ExeStatement(whileLoop, sm);
                        }
                    }
                    else
                        break;
                }
            }
            else if (stmt is FunctionCallStatement)
            {
                return ExeFunctionCall(block, (FunctionCall)stmt.Expression);
            }

            return null;
        }


        public object ExeExpression(CodeBlock block, Expression exp)
        {
            if (exp == null)
                return null;
            switch (exp.ExpType)
            {
                case ExpressionType.Assignment:
                    return ExeAssignment(block, (Assignment)exp);

                case ExpressionType.BinaryExpression:
                    return ExeBinaryExpression(block, (BinaryExpression)exp);


                case ExpressionType.Identifier:
                    return GetIdentifierValue(block, (Identifier)exp);
                case ExpressionType.Literal:
                    return ((Literal)exp).Value;

                case ExpressionType.FunctionCall:
                    return ExeFunctionCall(block, (FunctionCall)exp);

                case ExpressionType.UnaryExpression:
                    return ExeUnaryExpression(block, (UnaryExpression)exp);

                default:
                    throw new Exception("Invalid expression type: " + exp.ExpType.ToString());
            }

        }

        object ExeAssignment(CodeBlock block, Assignment exp)
        {
            string name = ((Identifier)exp.Variable).Name;

            object val = ExeExpression(block, exp.Rhs);

            block.AssignVar(name, TypedObject.Parse(val));

            return val;
        }

        object ExeUnaryExpression(CodeBlock block, UnaryExpression exp)
        {
            object val = ExeExpression(block, exp.Exp);

            switch (exp.Op)
            {
                case UnaryOperator.Plus:
                    return +(double)val;
                case UnaryOperator.Minus:
                    return -(double)val;
                default:
                    throw new Exception("Invalid unary operator: " + exp.Op.ToString());
            }
        }

        object ExeBinaryExpression(CodeBlock block, BinaryExpression exp)
        {
            object lhs = ExeExpression(block, exp.Lhs);
            object rhs = ExeExpression(block, exp.Rhs);
            if (lhs == null)
                lhs = String.Empty;
            if (rhs == null)
                rhs = String.Empty;

            switch (exp.Operator)
            {
                case BinaryOperator.Add:
                    return Add(lhs, rhs);
                case BinaryOperator.Substract:
                    return Substract(lhs, rhs);
                case BinaryOperator.Multiply:
                    return Multiply(lhs, rhs);
                case BinaryOperator.Divide:
                    return Divide(lhs, rhs);
                case BinaryOperator.Modulo:
                    return (int)lhs % (int)rhs;

                case BinaryOperator.Equal:
                case BinaryOperator.NotEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                    return Compare(exp.Operator, lhs, rhs);
                case BinaryOperator.And:
                    return And(lhs, rhs);
                case BinaryOperator.Or:
                    return Or(lhs, rhs);
                default:
                    throw new Exception("Invalid binary operator" + exp.Operator.ToString());
            }
        }
        object Or(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();
            
            if (lt != typeof(bool) || rt != typeof(bool))
                throw new Exception(String.Format("Operator '||' cannot be applied to operands of type '{0}' and '{1}'",lt.Name,rt.Name));
            else
                return (bool)lhs || (bool)rhs;
        }
        object And(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();

            if (lt != typeof(bool) || rt != typeof(bool))
                throw new Exception(String.Format("Operator '&&' cannot be applied to operands of type '{0}' and '{1}'", lt.Name, rt.Name));
            else
                return (bool)lhs && (bool)rhs;
        }
        object Divide(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();
            
            if (lt == typeof(string) || rt == typeof(string) ||
                lt == typeof(bool) || rt == typeof(bool))
                throw new Exception(String.Format("Operator '/' cannot be applied to operands of type '{0}' and '{1}'", lt.Name, rt.Name));            
            else if (lt == typeof(double) || rt == typeof(double))
                return double.Parse(lhs.ToString()) / double.Parse(rhs.ToString());
            else// if (lt == typeof(int) && rt == typeof(int))
                return (int)lhs / (int)rhs;
        }
        object Multiply(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();

            if (lt == typeof(string) || rt == typeof(string) ||
                lt==typeof(bool) || rt==typeof(bool))
                throw new Exception(String.Format("Operator '*' cannot be applied to operands of type '{0}' and '{1}'", lt.Name, rt.Name));            
            else if (lt == typeof(double) || rt == typeof(double))
                return double.Parse(lhs.ToString()) * double.Parse(rhs.ToString());
            else// if (lt == typeof(int) && rt == typeof(int))
                return (int)lhs * (int)rhs;

        }
        object Substract(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();

            if (lt == typeof(string) || rt == typeof(string) ||
                lt == typeof(bool) || rt == typeof(bool))
                throw new Exception(String.Format("Operator '-' cannot be applied to operands of type '{0}' and '{1}'", lt.Name, rt.Name));            
            else if (lt == typeof(double) || rt == typeof(double))
                return double.Parse(lhs.ToString()) - double.Parse(rhs.ToString());
            else// if (lt == typeof(int) && rt == typeof(int))
                return (int)lhs - (int)rhs;

        }
        object Add(object lhs, object rhs)
        {
            Type lt = lhs.GetType();
            Type rt = rhs.GetType();
            
            if (lt == typeof(string) || rt == typeof(string))
                return lhs.ToString() + rhs.ToString();
            else if (lt == typeof(bool) || rt == typeof(bool))
                throw new Exception(String.Format("Operator '+' cannot be applied to operands of type '{0}' and '{1}'", lt.Name, rt.Name));            
            else if (lt == typeof(double) || rt == typeof(double))
                return double.Parse(lhs.ToString()) + double.Parse(rhs.ToString());
            else// if (lt == typeof(int) && rt == typeof(int))
                return (int)lhs + (int)rhs;

        }
        bool Compare(BinaryOperator op, object lhs, object rhs)
        {
            IComparable lo = (IComparable)lhs;
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

        object GetIdentifierValue(CodeBlock block, Identifier exp)
        {
            TypedObject value = block.GetVar(exp.Name);


            Y2LArrayItem item = exp as Y2LArrayItem;
            if (item != null)
            {
                Y2LArray array = value as Y2LArray;
                return array[(int)ExeExpression(block, item.IndexExpression)];
            }
            return value.Value;
        }

        public object ExeFunctionCall(CodeBlock block, FunctionCall myFunc)
        {
            if (myFunc is CSharpFunctionCall)
            {
                object obj = null;
                
                if (myFunc.Args != null)
                {
                    // only one parameter
                    object param = ExeExpression(block, myFunc.Args[0]);
                    string strParam=param.ToString();
                    if (param.GetType() == typeof(string))
                        strParam = "\"" + strParam + "\"";

                    obj = CodeDomExecutor.ExecuteLines(myFunc.Name + "(" + strParam + ")");
                }
                else
                    obj = CodeDomExecutor.ExecuteLines(myFunc.Name + "()");

                return obj;
            }
            else if (myFunc is ReadLine)
            {
                return Console.ReadLine();
            }
            else if (_module.Functions.Contains(new Function(myFunc.Name)))
            {
                Function f = _module.GetFunction(myFunc.Name);

                if (myFunc.Args != null)
                {
                    for (int i = 0; i < myFunc.Args.Count; i++)
                    {
                        if (f.Parameters[i].IsArray)
                        {
                            string name = myFunc.Args[i].ToString();

                            Y2LArray array = (Y2LArray)block.GetVar(name);
                            if (array.PrimitiveType != f.Parameters[i].Type)
                            {
                                throw new Exception(String.Format("Cannot implicitly convert type '{0}' to '{1}'",
                                                                  array.PrimitiveType, f.Type));
                            }
                            f.AddVar(f.Parameters[i].Name, array);
                        }
                        else
                        {
                            f.AddVar(f.Parameters[i].Name, f.Parameters[i].Type);
                            object obj = ExeExpression(block, myFunc.Args[i]);
                            TypedObject tobj = TypedObject.Parse(obj);

                            if (tobj.PrimitiveType != PrimitiveType.Null && tobj.PrimitiveType != f.Parameters[i].Type)
                            {
                                throw new Exception(String.Format("Cannot implicitly convert type '{0}' to '{1}'",
                                                                  tobj.PrimitiveType, f.Type));
                            }
                            f.AssignVar(f.Parameters[i].Name, tobj);
                        }
                    }
                }
                return Execute(f);
            }
            throw new Exception("The function '"+ myFunc.Name+"' does not exist");
        }

    }
}
