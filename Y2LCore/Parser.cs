using System.Collections.Generic;
using System.Text;
using System;
using Y2LCore.Y2LMath;

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
        public Parser(Scanner scanner)
        {
            this._scanner = scanner;
            _errors = new List<SyntaxError>();
        }
        public Module Parse()
        {
            _module = this.ParseModule();
            Next();
            if (_token.Type != TokenType.EOF)
                throw new Exception("EOF expected");

            return _module;
        }
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
                _errors.Add(new SyntaxError("Thiếu '" + Token.TokenStrings[(int)tokenType] + "'", _token.Line, _token.Column));
            return true;
        }
        private bool Check(Token token)
        {
            Next();

            if (_token.Type != token.Type)
                _errors.Add(new SyntaxError("Thiếu '" + token + "'", _token.Line, _token.Column));
            return true;
        }
        public Module ParseModule()
        {
            Check(Token.Module);

            Module result;

            Check(TokenType.Identifier);

            result = new Module(_token.Text);

            Check(Token.LBracket);

            Token token = Peek();
            while (token.Type == TokenType.Function)
            {
                Next();
                result.Functions.Add(ParseFunction());
                token = Peek();
                //if (token.Type == TokenType.RBracket)
                //{
                //    Next();
                //    token = Peek();
                //}
            }
            Check(Token.RBracket);

            return result;
        }

        private Function ParseFunction()
        {
            Function result = new Function();
            Check(TokenType.PrimitiveToken);
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
                    result.Parameters = new List<Parameter>();

                Parameter para = new Parameter(_token.Text, pType);                
                
                result.Parameters.Add(para);
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

        private List<Statement> ParseStatements(Function func)
        {
            List<Statement> result = new List<Statement>();
            while (true)
            {
                Statement stmt = ParseStatement(func);
                if (stmt != null)
                    result.Add(stmt);

                Token tk = Peek();
                if (tk.Type == TokenType.RBracket)
                {
                    break;
                }
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
                    _errors.Add(new SyntaxError("Khong the gan kieu '" + lt.PrimitiveType + "' cho kieu '" + type + "' ", _token.Line, 0));
                    return false;
                }
            }
            else if (expr is ComparisionExpression)
            {
                if (type != PrimitiveType.Boolean)
                {
                    _errors.Add(new SyntaxError("Khong the gan kieu 'Boolean' cho kieu '" + type + "' ", _token.Line, 0));
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
        private Statement ParseStatement(Function func)
        {

            Statement result = null;
            // <stmt> := print <expr> 

            // <expr> := <string>
            // | <int>
            // | <arith_expr>
            // | <ident>

            Next();

            if (_token is PrimitiveToken)
            {
                DeclareVar declareVar = new DeclareVar();
                declareVar.PrimitiveType = ((PrimitiveToken)_token).PrimitiveType;

                Check(TokenType.Identifier);

                declareVar.Ident = _token.Text;
                func.AddVar(declareVar.Ident, new TypedObject(declareVar.PrimitiveType, null));
                Next();

                if (_token.Type == TokenType.Assign)
                {
                    declareVar.Expression = this.ParseExpr();
                    CheckValidAssign(declareVar.PrimitiveType, declareVar.Expression);
                    Check(TokenType.SemiColon);
                }

                result = declareVar;
            }
            else if (_token.Type == TokenType.Return)
            {
                result = new Return();
                result.Expression = ParseExpr();
                Check(TokenType.SemiColon);
            }
            else if (_token.Type == TokenType.Write)
            {
                result = new Print();
                Check(TokenType.LParen);
                Next();
                result.Expression = ReadExpressionList()[0];
                if(_token.Type!=TokenType.RParen)
                    Check(TokenType.RParen);
                if (_token.Type != TokenType.SemiColon)
                    Check(TokenType.SemiColon);
            }

            else if (_token.Type == TokenType.For)
            {
                Check(Token.LParen);
                ForLoop forLoop = new ForLoop();

                forLoop.From = ParseStatement(func);

                forLoop.Condition = ParseExpr();
                Check(Token.SemiColon);
                forLoop.Action = this.ParseStatement(func);
                forLoop.Parent = func;

                Check(Token.RParen);
                Check(Token.LBracket);

                forLoop.Statements = this.ParseStatements(func);
                result = forLoop;

                Check(Token.RBracket);
            }
            else if (_token.Type == TokenType.Identifier)
            {

                Token tk = Peek();
                // function call
                if (tk.Type == TokenType.LParen)
                {
                    FunctionCallStmt funcCall = new FunctionCallStmt();
                    funcCall.Expression = PrimaryExpr();
                    result = funcCall;
                }
                else
                {
                    // assignment
                    Assign assign = new Assign();
                    assign.Ident = _token.Text;

                    Check(Token.Assign);

                    assign.Expression = this.ParseExpr();
                    result = assign;
                }
                Check(TokenType.SemiColon);
            }
            else
            {
                _errors.Add(new SyntaxError("Thừa '" + _token + "'", _token.Line, _token.Column));
            }


            return result;

        }

        private Expression ParseExpr()
        {
            Next();
            return AssignmentExpr();
        }
        Expression AssignmentExpr()
        {
            Expression lhs = CompareExpr();

            if (_token.Type == TokenType.Assign)
            {
                Next(); // consume ASSIGN

                Expression rhs = AssignmentExpr();

                lhs = new Assignment(lhs, rhs);
            }

            return lhs;

        }
        Expression CompareExpr()
        {
            Expression lhs = AddExpr();

            if (_token.Type == TokenType.Equal ||
                _token.Type == TokenType.GreaterThan || _token.Type == TokenType.LessThan ||
                _token.Type == TokenType.GreaterThanOrEqual || _token.Type == TokenType.LessThanOrEqual)
            {
                TokenType type = _token.Type;

               // Next();
                Expression rhs=ParseExpr();

                BinaryOperator op=BinaryOperator.Equal;
                switch (type)
                {
                    case TokenType.Equal:
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
                    default:
                        _errors.Add(new SyntaxError("'" + type + "' khong phai la toan tu so sanh", _token.Line, _token.Column));
                        break;
                }
                lhs = new ComparisionExpression(lhs, op, rhs);

            }

            return lhs;
        }
        Expression AddExpr()
        {
            Expression lhs = TimesExpr();

            while (_token.Type == TokenType.Plus || _token.Type == TokenType.Minus)
            {
                BinaryOperator op = _token.Type == TokenType.Plus ? BinaryOperator.Plus : BinaryOperator.Minus;

                Next(); // read + or -

                Expression rhs = TimesExpr();

                lhs = new BinaryExpression(lhs, op, rhs);

            }

            return lhs;
        }

        Expression TimesExpr()
        {
            Expression lhs = UnaryExpr();

            while (_token.Type == TokenType.Multiply || _token.Type == TokenType.Divide)
            {
                BinaryOperator op = _token.Type == TokenType.Multiply ? BinaryOperator.Times : BinaryOperator.Divide;

                Next(); // read + or -

                Expression rhs = UnaryExpr();

                lhs = new BinaryExpression(lhs, op, rhs);

            }

            return lhs;
        }

        Expression UnaryExpr()
        {
            Expression ret = null;

            if (_token.Type == TokenType.Plus || _token.Type == TokenType.Minus)
            {
                int line = _token.Line;
                int col = _token.Column;
                UnaryOperator op = _token.Type == TokenType.Plus ? UnaryOperator.Plus : UnaryOperator.Minus;

                Next();	// read + or -

                Expression exp = UnaryExpr();

                ret = new UnaryExpression(op, exp);
            }
            else
                ret = PrimaryExpr();

            return ret;
        }

        Expression PrimaryExpr()
        {
            Expression ret = null;

            if (_token.Type == TokenType.StringLiteral)
            {
                ret = new StringLiteral(_token.Text);
                Token tk = Peek();
                if (tk.Type != TokenType.SemiColon && tk.Type != TokenType.RParen)
                    Next();
            }
            else if (_token.Type == TokenType.True || _token.Type==TokenType.False)
            {
                ret = new BooleanLiteral(_token.Type==TokenType.True);
                Token tk = Peek();
                if (tk.Type != TokenType.SemiColon && tk.Type != TokenType.RParen)
                    Next();
            }
            else if (_token.Type == TokenType.Number)
            {
                if (_token.Text.Contains("."))
                    ret = new DoubleLiteral(double.Parse(_token.Text));
                else
                    ret = new IntLiteral(int.Parse(_token.Text));

                Token tk = Peek();
                if (tk.Type != TokenType.SemiColon && tk.Type != TokenType.RParen)
                    Next();
            }
            else if (_token.Type == TokenType.Identifier)
            {
                string name = _token.Text;
                ret = new Identifier(name);

                Token tk = Peek();
                if (tk.Type != TokenType.SemiColon)
                {
                    Next();
                    if (_token.Type == TokenType.LParen)
                    {
                        // function call
                        Next(); // read (

                        ExpressionList args = ReadExpressionList();
                       // if (_token.Type != TokenType.RParen)
                            Check(TokenType.RParen);

                        ret = new FunctionCall(((Identifier)ret).Name, args);
                    }
                }
                //if (Peek().Type != TokenType.SemiColon)
                //    Next();
            }
            else if (_token.Type == TokenType.LParen)
            {
                Next();

                ret = AddExpr();

                Check(TokenType.RParen);

                if (Peek().Type != TokenType.SemiColon)
                    Next();
            }
            else
                _errors.Add(new SyntaxError("Unexpected _token found " + _token.Type.ToString() + " (" + _token.Text + ")", _token.Line, _token.Column));

            return ret;
        }

        ExpressionList ReadExpressionList()
        {
            if (_token.Type == TokenType.RParen)
                return null;
            ExpressionList list = new ExpressionList();
            Token tk = _token;
             while (tk.Type != TokenType.RParen)
            {

                Expression exp = AddExpr();

                list.Add(exp);


                tk = Peek();
                if (_token.Type == TokenType.Comma)
                {
                    Next();
                }
                else if (_token.Type == TokenType.RParen)
                    break;
            }

            return list;
        }
    }
}