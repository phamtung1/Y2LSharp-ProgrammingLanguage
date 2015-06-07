using System.Collections.Generic;
using System.Text;
using System;
using Y2LCore.SyntaxTree;
using System.Text.RegularExpressions;


namespace Y2LCore
{
    public sealed class Parser
    {

        private Scanner _scanner;
        private Token _token;
        private Module _module;
        private List<SyntaxError> _errors;
        public List<SyntaxError> Errors
        {
            get { return _errors; }
        }

        #region Constructors

        public Parser(Scanner scanner)
        {
            this._scanner = scanner;
            _errors = new List<SyntaxError>();
        }
        public Module Parse()
        {
            _module = this.ParseModule();
            Check(TokenType.EOF);

            return _module;
        }
        #endregion

        private Token Peek()
        {
            return _scanner.Peek();
        }
        private void Next()
        {
            _token = _scanner.Next();
        }
        private bool Check(TokenType tokenType)
        {
            Next();
            if (_token.Type != tokenType)
            {
                _errors.Add(new SyntaxError("'" + tokenType + "' expected", _token.Line, _token.Column));
                Next();
            }
            return true;
        }
        private bool Check(Token token)
        {
            Next();

            if (_token.Type != token.Type)
            {
                _errors.Add(new SyntaxError("'" + token + "' expected", _token.Line, _token.Column));
                Next();
            }
            return true;
        }
        public Module ParseModule()
        {
            Check(Token.Module);

            Module result;

            Check(TokenType.Identifier);

            result = new Module(_token.Text);

            Check(Token.LBracket);

            Token tk = Peek();
            while (tk.Type != TokenType.EOF && tk.Type != TokenType.RBracket)
            {

                if (tk.Type == TokenType.Function || tk.Type == TokenType.PrimitiveToken)
                {
                    if (tk.Type == TokenType.Function)
                    {
                        Next();
                        result.Functions.Add(ParseFunction(result));
                    }
                    else if (tk.Type != TokenType.LineComment)
                    {
                        Statement stmt = ParseStatement(result);
                        result.Statements.Add(stmt);
                    }

                }
                else
                {
                    _errors.Add(new SyntaxError("Unrecognized token " + _token, _token.Line, _token.Column));
                    Next();
                }
                // Next();
                tk = Peek();
            }
            //if (_token.Type != TokenType.RBracket)
            //    _errors.Add(new SyntaxError("Thieu }", _token.Line, _token.Column));
            Check(TokenType.RBracket);
            return result;
        }

        private Function ParseFunction(Module parent)
        {
            Function result = new Function();
            result.Parent = parent;

            Next();
            if (!(_token is PrimitiveToken))
            {
                Errors.Add(new SyntaxError("Unrecognized data type'" + _token.Text + "'", _token.Line, _token.Column));
                result.Type = PrimitiveType.Unknown;
            }
            else
                result.Type = ((PrimitiveToken)_token).PrimitiveType;

            Check(TokenType.Identifier);
            result.Name = _token.Text;
            Check(Token.LParen);

            Token token = Peek();
            // Parameters
            while (token is PrimitiveToken)
            {
                Next();
                PrimitiveType pType = ((PrimitiveToken)_token).PrimitiveType;

                Check(TokenType.Identifier);
                if (result.Parameters == null)
                    result.Parameters = new List<FormalParameter>();

                FormalParameter param = new FormalParameter(_token.Text, pType);

                token = Peek();
                if (token.Type == TokenType.LSquareBracket)
                {
                    Next();
                    Check(TokenType.RSquareBracket);
                    param.IsArray = true;
                }

                result.Parameters.Add(param);

                token = Peek();
                if (token.Type == TokenType.Comma)
                {
                    Next();
                    token = Peek();
                }
            }

            Check(Token.RParen);

            Check(Token.LBracket);

            result.Statements = ParseStatements(result);

            Check(Token.RBracket);

            return result;
        }

        private List<Statement> ParseStatements(CodeBlock func)
        {
            List<Statement> result = new List<Statement>();
            while (true)
            {
                Statement stmt = ParseStatement(func);
                if (stmt != null)
                    result.Add(stmt);

                Token tk = Peek();
                if (tk.Type == TokenType.RBracket || tk.Type == TokenType.EOF)
                    break;
                //		Next();
            }
            return result;
        }

        bool CheckValidAssign(PrimitiveType type, Expression expr)
        {
            if (expr is Literal)
            {
                Literal lt = (Literal)expr;
                if (type != lt.PrimitiveType)
                {
                    _errors.Add(new SyntaxError("Cannot implicitly convert type '" + lt.PrimitiveType + "' to '" + type + "'", _token.Line, 0));
                    return false;
                }
            }
            else if (expr is ComparisionExpression)
            {
                if (type != PrimitiveType.LuanLy)
                {
                    _errors.Add(new SyntaxError("Cannot implicitly convert type 'bool' to '" + type + "' ", _token.Line, 0));
                    return false;
                }
            }
            else if (expr is BinaryExpression)
            {
                BinaryExpression binExpr = (BinaryExpression)expr;

                bool b = CheckValidAssign(type, binExpr.Lhs);
                if (b)
                    b = CheckValidAssign(type, binExpr.Rhs);
                return b;
            }
            return true;
        }

        private Statement ParseStatement(CodeBlock block)
        {

            Statement result = null;

            Next();

            #region Declare Variable

            if (_token is PrimitiveToken)
            {

                DeclareVariable dec = null;
                PrimitiveType pType = ((PrimitiveToken)_token).PrimitiveType;

                Check(TokenType.Identifier);

                string ident = _token.Text;

                Next();

                if (_token.Type == TokenType.Assign || _token.Type == TokenType.SemiColon)
                {
                    block.AddVar(ident, pType);
                    dec = new DeclareVariable(pType);
                    if (_token.Type == TokenType.Assign)
                    {
                        dec.Expression = this.ParseExpression();
                        CheckValidAssign(dec.Type, dec.Expression);
                        Check(TokenType.SemiColon);
                    }
                }
                // array
                else if (_token.Type == TokenType.LSquareBracket)
                {
                    dec = new ArrayDeclare(pType);
                    Y2LArray array = new Y2LArray(pType);
                    block.AddVar(ident, array);

                    dec.Expression = this.ParseExpression();
                    array.LengthExpression = dec.Expression;

                    Check(TokenType.RSquareBracket);
                    Check(TokenType.SemiColon);
                }
                else
                    _errors.Add(new SyntaxError("Unrecognized token '" + _token.Text + "'", _token.Line, _token.Column));

                if (dec != null)
                {
                    dec.Ident = ident;
                    //	dec.Type = pType;
                }
                result = dec;

            }
            #endregion

            else if (_token.Type == TokenType.Return)
            {
                result = new Return();
                result.Expression = ParseExpression();
                Check(TokenType.SemiColon);
            }
            else if (_token.Type == TokenType.Write)
            {
                result = new PrintStatement();
                Check(TokenType.LParen);
                Next();
                result.Expression = ReadFunctionCallParameters()[0];
                Check(TokenType.RParen);
                Check(TokenType.SemiColon);
            }
            else if (_token.Type == TokenType.ReadLine)
            {
                result = new ReadLineStatement();
                Check(TokenType.LParen);
                Check(TokenType.RParen);
                Check(TokenType.SemiColon);
            }
            #region ForLoop
            else if (_token.Type == TokenType.For)
            {
                Check(Token.LParen);
                ForLoop forLoop = new ForLoop();

                forLoop.From = ParseStatement(block);

                forLoop.Condition = ParseExpression();
                Check(Token.SemiColon);
                forLoop.Action = this.ParseStatement(block);
                forLoop.Parent = block;

                Check(Token.RParen);
                Check(Token.LBracket);

                forLoop.Statements = this.ParseStatements(forLoop);
                result = forLoop;

                Check(Token.RBracket);
            }
            #endregion

            #region WhileLoop

            else if (_token.Type == TokenType.While)
            {
                WhileLoop whileLoop = new WhileLoop();

                Check(TokenType.LParen);
                whileLoop.Condition = ParseExpression();
                Check(TokenType.RParen);

                whileLoop.Parent = block;

                Check(Token.LBracket);

                whileLoop.Statements = this.ParseStatements(whileLoop);
                result = whileLoop;

                Check(Token.RBracket);
            }
            #endregion

            #region IfElse

            else if (_token.Type == TokenType.If)
            {
                Check(TokenType.LParen);
                IfElse ifElse = new IfElse();
                ifElse.IfBlock.Parent = block;
                ifElse.Condition = ParseExpression();

                Next();
                Check(TokenType.LBracket);
                ifElse.IfBlock.Statements = this.ParseStatements(ifElse.IfBlock);
                Check(TokenType.RBracket);
                Token tk = Peek();
                if (tk.Type == TokenType.ElseIf)
                {
                    Next();
                    ifElse.ElseIfStatements = new List<IfElse>();

                    while (_token.Type == TokenType.ElseIf)
                    {
                        Check(TokenType.LParen);
                        IfElse elseIf = new IfElse();
                        elseIf.IfBlock.Parent = block;
                        elseIf.Condition = ParseExpression();
                        Check(TokenType.RParen);
                        Check(TokenType.LBracket);
                        elseIf.IfBlock.Statements = this.ParseStatements(elseIf.IfBlock);
                        Check(TokenType.RBracket);
                        ifElse.ElseIfStatements.Add(elseIf);
                        Next();
                        tk = Peek();
                    }
                }

                if (tk.Type == TokenType.Else || _token.Type == TokenType.Else)
                {
                    if (_token.Type != TokenType.Else)
                        Next();

                    Check(TokenType.LBracket);
                    ifElse.ElseBlock = new CodeBlock();
                    ifElse.ElseBlock.Parent = block;
                    ifElse.ElseBlock.Statements = this.ParseStatements(ifElse.ElseBlock);
                    Check(TokenType.RBracket);
                }
                result = ifElse;
            }
            #endregion

            #region FunctionCall + Assignment

            else if (_token.Type == TokenType.Identifier || _token.Type == TokenType.CsharpIns)
            {
                bool isCs = _token.Type == TokenType.CsharpIns;
                Token tk = Peek();
                // function call
                if (tk.Type == TokenType.LParen)
                {
                    string name = _token.Text;
                    Next();
                    Next();
                    Statement funcCall;
                    if (isCs)
                    {
                        funcCall = new CSharpFunctionCallSatement();
                        funcCall.Expression = new CSharpFunctionCall(name, ReadFunctionCallParameters());
                    }
                    else
                    {
                        funcCall = new FunctionCallStatement();
                        funcCall.Expression = new FunctionCall(name, ReadFunctionCallParameters());
                    }
                    result = funcCall;
                    Check(TokenType.RParen);
                }
                else
                {
                    // assignment
                    AssignSatement assign = null;
                    string ident = _token.Text;
                    tk = Peek();

                    if (tk.Type == TokenType.LSquareBracket)
                    {
                        Next();
                        assign = new ArrayItemAssignSatement();
                        ((ArrayItemAssignSatement)assign).IndexExpression = this.ParseExpression();
                        Check(TokenType.RSquareBracket);
                    }
                    else
                    {
                        assign = new AssignSatement();
                    }

                    assign.Ident = ident;

                    Check(Token.Assign);

                    assign.Expression = this.ParseExpression();

                    result = assign;
                }

                Check(TokenType.SemiColon);
            }
            #endregion

            else if (_token.Type != TokenType.LineComment)
            {
                _errors.Add(new SyntaxError("Unrecognized token '" + _token + "'", _token.Line, _token.Column));
                //Next();
            }
            return result;
        }

        #region Parse Expression

        private Expression ParseExpression()
        {
            Next();
            return AssignExpression();
        }


        Expression AssignExpression()
        {
            Expression lhs = CompareExpression();

            if (_token.Type == TokenType.Assign)
            {
                Next();

                Expression rhs = AssignExpression();

                lhs = new Assignment(lhs, rhs);
            }

            return lhs;

        }
        // Compare and Boolean
        Expression CompareExpression()
        {
            Expression lhs = AddExpression();

            if (_token.Type == TokenType.Equal ||
                _token.Type == TokenType.NotEqual ||
                _token.Type == TokenType.GreaterThan || _token.Type == TokenType.LessThan ||
                _token.Type == TokenType.GreaterThanOrEqual ||
                _token.Type == TokenType.LessThanOrEqual ||
                _token.Type == TokenType.And || _token.Type == TokenType.Or
               )
            {
                TokenType type = _token.Type;

                // Next();
                Expression rhs = ParseExpression();

                BinaryOperator op = BinaryOperator.Equal;
                switch (type)
                {
                    case TokenType.Equal:
                        op = BinaryOperator.Equal;
                        break;
                    case TokenType.NotEqual:
                        op = BinaryOperator.Equal;
                        break;
                    case TokenType.GreaterThan:
                        op = BinaryOperator.GreaterThan;
                        break;
                    case TokenType.GreaterThanOrEqual:
                        op = BinaryOperator.GreaterThanOrEqual;
                        break;
                    case TokenType.LessThan:
                        op = BinaryOperator.LessThan;
                        break;
                    case TokenType.LessThanOrEqual:
                        op = BinaryOperator.LessThanOrEqual;
                        break;
                    case TokenType.And:
                        op = BinaryOperator.And;
                        break;
                    case TokenType.Or:
                        op = BinaryOperator.Or;
                        break;
                    default:

                        break;
                }
                lhs = new ComparisionExpression(lhs, op, rhs);

            }

            return lhs;
        }
        Expression AddExpression()
        {
            Expression lhs = MultExpression();

            while (_token.Type == TokenType.Plus || _token.Type == TokenType.Minus)
            {
                BinaryOperator op = _token.Type == TokenType.Plus ? BinaryOperator.Add : BinaryOperator.Substract;

                Next();

                Expression rhs = MultExpression();

                lhs = new BinaryExpression(lhs, op, rhs);

            }

            return lhs;
        }

        Expression MultExpression()
        {
            Expression lhs = UnaryExpression();

            while (_token.Type == TokenType.Multiply
                   || _token.Type == TokenType.Divide
                   || _token.Type == TokenType.Modulo)
            {
                BinaryOperator op;
                if (_token.Type == TokenType.Multiply)
                    op = BinaryOperator.Multiply;
                else if (_token.Type == TokenType.Divide)
                    op = BinaryOperator.Divide;
                else
                    op = BinaryOperator.Modulo;

                Next();

                Expression rhs = UnaryExpression();

                lhs = new BinaryExpression(lhs, op, rhs);

            }

            return lhs;
        }

        Expression UnaryExpression()
        {
            Expression ret = null;

            if (_token.Type == TokenType.Plus || _token.Type == TokenType.Minus)
            {
                int line = _token.Line;
                int col = _token.Column;
                UnaryOperator op = _token.Type == TokenType.Plus ? UnaryOperator.Plus : UnaryOperator.Minus;

                Next();

                Expression exp = UnaryExpression();

                ret = new UnaryExpression(op, exp);
            }
            else
                ret = PrimaryExpression();

            return ret;
        }

        Expression PrimaryExpression()
        {
            Expression ret = null;

            if (_token.Type == TokenType.StringLiteral)
            {
                ret = new StringLiteral(_token.Text);
                Token tk = Peek();
                if (!IsCloseBracketOrSemiColon(tk.Type))
                    Next();
            }
            else if (_token.Type == TokenType.True || _token.Type == TokenType.False)
            {
                ret = new BooleanLiteral(_token.Type == TokenType.True);
                Token tk = Peek();
                if (!IsCloseBracketOrSemiColon(tk.Type))
                    Next();
            }
            else if (_token.Type == TokenType.Number)
            {
                if (_token.Text.Contains("."))
                    ret = new DoubleLiteral(double.Parse(_token.Text));
                else
                    ret = new IntLiteral(int.Parse(_token.Text));

                Token tk = Peek();
                if (!IsCloseBracketOrSemiColon(tk.Type))
                    Next();
            }
            else if (_token.Type == TokenType.Identifier)
            {
                string name = _token.Text;

                Token tk = Peek();
                if (!IsCloseBracketOrSemiColon(tk.Type))
                {
                    Next();
                    if (_token.Type == TokenType.LParen)
                    {
                        // function call
                        Next();
                        if (_token.Type == TokenType.RParen)
                        {
                            ret = new FunctionCall(name);
                        }
                        else{
                            List<Expression> args = ReadFunctionCallParameters();

                            Check(TokenType.RParen);

                            ret = new FunctionCall(name, args);
                        }
                    }
                    else if (_token.Type == TokenType.LSquareBracket) // array
                    {

                        Y2LArrayItem item = new Y2LArrayItem(name);

                        item.IndexExpression = this.ParseExpression();
                        Check(TokenType.RSquareBracket);
                        tk = Peek();
                        if (!IsCloseBracketOrSemiColon(tk.Type))
                            Next();

                        ret = item;
                    }
                    else
                        ret = new Identifier(name);
                }
                else
                    ret = new Identifier(name);
            }
            else if (_token.Type == TokenType.CsharpIns)
            {

                string methodName = _token.Text;
                Check(TokenType.LParen);

                CSharpFunctionCall fc = null;
                if (Peek().Type == TokenType.RParen)
                    fc = new CSharpFunctionCall(methodName, null);
                else
                {
                    Next();
                    fc = new CSharpFunctionCall(methodName, ReadFunctionCallParameters());
                }

                Check(TokenType.RParen);
                ret = fc;
            }
            else if (_token.Type == TokenType.ReadLine)
            {
                ret = new ReadLine();
                Check(TokenType.LParen);
                Check(TokenType.RParen);
            }
            else if (_token.Type == TokenType.LParen)
            {
                Next();

                ret = CompareExpression();

                Check(TokenType.RParen);

                if (!IsCloseBracketOrSemiColon(Peek().Type))
                    Next();
            }
            else
            {
                _errors.Add(new SyntaxError("Unrecognized token '" + _token.Type.ToString() + "' (" + _token.Text + ")", _token.Line, _token.Column));
                Next();
            }

            return ret;
        }

        #endregion


        List<Expression> ReadFunctionCallParameters()
        {

            if (_token.Type == TokenType.RParen)
                return null;

            List<Expression> list = new List<Expression>();
            Token tk = _token;
            while (tk.Type != TokenType.RParen)
            {
                Expression exp = CompareExpression();

                list.Add(exp);

                tk = Peek();
                if (_token.Type == TokenType.Comma)
                {
                    Next();
                }
                else// if (_token.Type == TokenType.RParen)
                    break;

            }

            return list;
        }

        bool IsCloseBracketOrSemiColon(TokenType type)
        {
            return type == TokenType.RBracket ||
            type == TokenType.RParen ||
            type == TokenType.RSquareBracket ||
            type == TokenType.SemiColon;

        }
    }
}