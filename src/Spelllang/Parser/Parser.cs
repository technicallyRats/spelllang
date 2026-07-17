using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Spelllang.AST;
using Spelllang.Lexer;
using Type = Spelllang.Lexer.Type;

namespace Spelllang.Parser
{
    public delegate IExpressionNode ParsePrefixExpressionFn();

    public delegate IExpressionNode ParseInfixExpressionFn(IExpressionNode left);

    public delegate IStatementNode ParseStatementFn();

    //TODO: does this need to be public?
    public enum Precedence
    {
        PRECEDENCE_LOWEST,
        PRECEDENCE_ASSIGN,
        PRECEDENCE_EQUAL,
        PRECEDENCE_COMPARE,
        PRECEDENCE_STRICH,
        PRECEDENCE_PUNKT,
        PRECEDENCE_PREFIX,
        PRECEDENCE_CALL,
        PRECEDENCE_INDEX,

        PRECEDENCE_MAX
    }

    public class Parser
    {
        private readonly List<ParsingError> _parsingErrors = new();
        private readonly bool Done = false;

        private readonly TokenEnumerator LexerEnumerator;
        private Dictionary<Type, ParseInfixExpressionFn> InfixParserFn;
        private Dictionary<Type, Precedence> PrecedenceMapping;

        private Dictionary<Type, ParsePrefixExpressionFn> PrefixParserFn;
        private ProgramNode ProgramNode;

        public Parser(Lexer.Lexer lexer)
        {
            Setup();
            LexerEnumerator = lexer.GetEnumerator();
            Parse();
        }

        public Parser(TokenEnumerator lexerEnumerator)
        {
            Setup();
            LexerEnumerator = lexerEnumerator;
            Parse();
        }

        private void Setup()
        {
            PrefixParserFn = new Dictionary<Type, ParsePrefixExpressionFn>
            {
                { Type.NUMBER, ParseNumberExpression },
                { Type.STRING, ParseStringExpression },
                { Type.BOOLEAN, ParseBooleanExpression },
                { Type.IDENTIFIER, ParseIdentifierExpression },
                { Type.SEMICOLON, ParseSemicolonExpression },
                { Type.NULL, ParseNullExpression },
                { Type.MINUS, ParsePrefixExpression },
                { Type.PLUS, ParsePrefixExpression },
                { Type.NOT, ParsePrefixExpression }
            };

            InfixParserFn = new Dictionary<Type, ParseInfixExpressionFn>
            {
                { Type.EQUAL, ParseInfixExpression },
                { Type.NOT_EQUAL, ParseInfixExpression },
                { Type.GT, ParseInfixExpression },
                { Type.GTE, ParseInfixExpression },
                { Type.LT, ParseInfixExpression },
                { Type.LTE, ParseInfixExpression },
                { Type.PLUS, ParseInfixExpression },
                { Type.MINUS, ParseInfixExpression },
                { Type.MULTIPLY, ParseInfixExpression },
                { Type.DIVIDE, ParseInfixExpression },
                { Type.MODULO, ParseInfixExpression },
                { Type.AND, ParseInfixExpression },
                { Type.OR, ParseInfixExpression },
                { Type.ASSIGN, ParseAssignExpression },
                { Type.PARENTHESES_LEFT, ParseCallExpression }
            };

            PrecedenceMapping = new Dictionary<Type, Precedence>
            {
                { Type.ASSIGN, Precedence.PRECEDENCE_ASSIGN },
                { Type.MINUS, Precedence.PRECEDENCE_STRICH },
                { Type.PLUS, Precedence.PRECEDENCE_STRICH },
                { Type.MULTIPLY, Precedence.PRECEDENCE_PUNKT },
                { Type.DIVIDE, Precedence.PRECEDENCE_PUNKT },
                { Type.MODULO, Precedence.PRECEDENCE_PUNKT },
                { Type.EQUAL, Precedence.PRECEDENCE_EQUAL },
                { Type.AND, Precedence.PRECEDENCE_EQUAL },
                { Type.OR, Precedence.PRECEDENCE_EQUAL },
                { Type.NOT_EQUAL, Precedence.PRECEDENCE_EQUAL },
                { Type.GT, Precedence.PRECEDENCE_COMPARE },
                { Type.GTE, Precedence.PRECEDENCE_COMPARE },
                { Type.LT, Precedence.PRECEDENCE_COMPARE },
                { Type.LTE, Precedence.PRECEDENCE_COMPARE },
                { Type.PARENTHESES_LEFT, Precedence.PRECEDENCE_CALL }
            };

            ProgramNode = new ProgramNode();
        }

        public ProgramNode GetRootProgram()
        {
            return ProgramNode;
        }

        public bool IsFaulty()
        {
            return _parsingErrors.Count > 0;
        }

        public void DisplayErrors(string? sourceCode)
        {
            foreach (var error in _parsingErrors) Console.WriteLine(error.Show(sourceCode));
        }

        private void Parse()
        {
            var statements = new List<IStatementNode>();
            while (!Done && !CurrentItemTypeEquals(Type.EOF) && !CurrentItemTypeEquals(Type.BRACES_RIGHT))
            {
                var statement = ParseStatement();
                if (statement != null) statements.Add(statement);
            }

            ProgramNode.SetStatements(statements);
        }

        private IStatementNode? ParseStatement()
        {
            var result = CurrentItemType() switch
            {
                Type.RETURN => ParseReturnStatement(),
                Type.FUNCTION => ParseFunctionStatement(),
                Type.IF => ParseIfStatement(),
                Type.WHILE => ParseWhileStatement(),
                Type.BREAK => ParseBreakStatement(),
                Type.IMPORT => ParseImportStatement(),
                _ => new ExpressionStatement(ParseExpression(Precedence.PRECEDENCE_LOWEST))
            };

            LexerEnumerator.Next();
            return result;
        }

        private IExpressionNode ParseExpression(Precedence precedence)
        {
            var parsePrefixExpressionFn =
                PrefixParserFn.TryGetValue(LexerEnumerator.Current().Type, out var v) ? v : null;
            if (parsePrefixExpressionFn == null)
            {
                ReportError("No Prefix parser for this token", LexerEnumerator.Current());
                return new NullExpression();
            }

            var leftExpression = parsePrefixExpressionFn();

            while (!PeekItemTypeEquals(Type.EOF) && precedence < PeekItemPrecedence())
            {
                var infixParser = InfixParserFn.TryGetValue(LexerEnumerator.Peek().Type, out var value) ? value : null;
                if (infixParser == null)
                {
                    ReportError("No Infix parser for this token", LexerEnumerator.Peek());
                    return leftExpression;
                }

                LexerEnumerator.Next();

                leftExpression = infixParser(leftExpression);
            }

            return leftExpression;
        }

        private IExpressionNode ParseInfixExpression(IExpressionNode left)
        {
            var op = LexerEnumerator.Current().Value;

            var precedence = CurrItemPrecedence();
            LexerEnumerator.Next();

            var right = ParseExpression(precedence);

            return new InfixExpression(left, right, op);
        }

        private IExpressionNode ParsePrefixExpression()
        {
            var op = LexerEnumerator.Current().Value;
            LexerEnumerator.Next();
            var expression = ParseExpression(Precedence.PRECEDENCE_PREFIX);
            return new PrefixExpression(op, expression);
        }

        private IExpressionNode ParseAssignExpression(IExpressionNode identifier)
        {
            var precedence = CurrItemPrecedence();
            LexerEnumerator.Next();

            var value = ParseExpression(precedence);
            if (identifier is IdentifierExpression identifierExpression)
                return new AssignExpression(identifierExpression.IdentifierName, value);
            ReportError("Expected identifier node instead", LexerEnumerator.Peek());
            return new NullExpression();
        }

        private IExpressionNode ParseNullExpression()
        {
            return new NullExpression();
        }

        private IExpressionNode ParseNumberExpression()
        {
            // Remove _ for syntactic sugar
            var rawValue = LexerEnumerator.Current().Value.Replace("_", "");

            if (rawValue.Contains(".")) return new FloatExpression(float.Parse(rawValue, CultureInfo.InvariantCulture));

            return new IntegerExpression(int.Parse(rawValue));
        }

        private IExpressionNode ParseStringExpression()
        {
            return new StringExpression(LexerEnumerator.Current().Value);
        }

        private IExpressionNode ParseBooleanExpression()
        {
            return new BooleanExpression(LexerEnumerator.Current().Value == "true");
        }

        private IExpressionNode ParseIdentifierExpression()
        {
            return new IdentifierExpression(LexerEnumerator.Current().Value);
        }

        private IExpressionNode ParseCallExpression(IExpressionNode left)
        {
            var arguments = ParseExpressionList(Type.PARENTHESES_RIGHT);
            return new CallExpression(left, arguments);
        }

        private List<IExpressionNode> ParseExpressionList(Type terminatingType)
        {
            var result = new List<IExpressionNode>();

            LexerEnumerator.Next();

            if (CurrentItemTypeEquals(terminatingType)) return result;

            while (!CurrentItemTypeEquals(terminatingType))
            {
                result.Add(ParseExpression(Precedence.PRECEDENCE_LOWEST));
                LexerEnumerator.Next();
                if (CurrentItemTypeEquals(Type.COMMA)) LexerEnumerator.Next();
            }

            return result;
        }

        private IExpressionNode ParseSemicolonExpression()
        {
            return new SemicolonExpression();
        }

        private IStatementNode ParseFunctionStatement()
        {
            CheckedNext(new List<Type> { Type.PARENTHESES_LEFT, Type.IDENTIFIER });
            var functionName = FunctionStatement.ANONYMOUS_FUNCTION_NAME;
            if (CurrentItemTypeEquals(Type.IDENTIFIER))
            {
                functionName = LexerEnumerator.Current().Value;
                LexerEnumerator.Next();
            }

            var arguments = ParseExpressionList(Type.PARENTHESES_RIGHT);
            if (arguments.Count > 0 && !arguments.All(item => item != null && item is IdentifierExpression))
            {
                ReportError("Expected identifiers as arguments but got " + arguments, LexerEnumerator.Current());
                return new NullStatement();
            }

            var argumentIdentifiers = arguments.Cast<IdentifierExpression>().ToList();

            LexerEnumerator.Next();
            var functionBody = ParseBlock();
            return new FunctionStatement(functionName, argumentIdentifiers, functionBody);
        }

        private IStatementNode ParseReturnStatement()
        {
            LexerEnumerator.Next();
            if (CurrentItemTypeEquals(Type.BRACES_RIGHT) || CurrentItemTypeEquals(Type.SEMICOLON))
                return new ReturnStatement(new NullExpression());
            return new ReturnStatement(ParseExpression(Precedence.PRECEDENCE_LOWEST));
        }

        private IStatementNode ParseIfStatement()
        {
            CheckedNext(Type.PARENTHESES_LEFT);
            LexerEnumerator.Next();
            var condition = ParseExpression(Precedence.PRECEDENCE_LOWEST);
            // skip )
            CheckedNext(Type.PARENTHESES_RIGHT);
            // skip {
            LexerEnumerator.Next();
            var primary = ParseBlock();
            var secondary = new ProgramNode();
            LexerEnumerator.Next();
            if (CurrentItemTypeEquals(Type.ELSE))
            {
                LexerEnumerator.Next();
                secondary = ParseBlock();
            }

            return new IfStatement(primary, secondary, condition);
        }

        private IStatementNode ParseWhileStatement()
        {
            CheckedNext(Type.PARENTHESES_LEFT);
            LexerEnumerator.Next();
            var condition = ParseExpression(Precedence.PRECEDENCE_LOWEST);
            // skip )
            CheckedNext(Type.PARENTHESES_RIGHT);
            // skip {
            LexerEnumerator.Next();
            var body = ParseBlock();
            return new WhileStatement(body, condition);
        }

        private IStatementNode ParseBreakStatement()
        {
            return new BreakStatement();
        }

        private IStatementNode ParseImportStatement()
        {
            CheckedNext(new List<Type>
            {
                Type.IDENTIFIER, Type.STRING
            }); // this allows strings, but I have never tested string literals here, oh well
            var importPath = LexerEnumerator.Current().Value; // assume this counts as identifier?
            var importName = "";
            if (PeekItemTypeEquals(Type.AS))
            {
                LexerEnumerator.Next();
                CheckedNext(Type.IDENTIFIER);
                importName = LexerEnumerator.Current().Value;
            }

            return new ImportStatement(importPath, importName);
        }

        private ProgramNode ParseBlock()
        {
            if (CurrentItemTypeEquals(Type.BRACES_LEFT)) LexerEnumerator.Next();
            return new Parser(LexerEnumerator).GetRootProgram();
        }

        private Precedence CurrItemPrecedence()
        {
            return PrecedenceMapping.TryGetValue(LexerEnumerator.Current().Type, out var value)
                ? value
                : Precedence.PRECEDENCE_LOWEST;
        }

        private Precedence PeekItemPrecedence()
        {
            return PrecedenceMapping.TryGetValue(LexerEnumerator.Peek().Type, out var value)
                ? value
                : Precedence.PRECEDENCE_LOWEST;
        }

        private bool PeekItemTypeEquals(Type expected)
        {
            return LexerEnumerator.Peek().Type == expected;
        }

        private Type CurrentItemType()
        {
            return LexerEnumerator.Current().Type;
        }

        private bool CurrentItemTypeEquals(Type expected)
        {
            return CurrentItemType() == expected;
        }

        private void ReportError(string description, Token token)
        {
            _parsingErrors.Add(new ParsingError(token.StartIndex, token.Value.Length, description));
        }

        private Token CheckedNext(Type expected)
        {
            if (!PeekItemTypeEquals(expected))
                ReportError("Expected type " + expected + " but got " + LexerEnumerator.Peek().Type,
                    LexerEnumerator.Peek());
            // We advance anyway because this possibly catches additional errors
            return LexerEnumerator.Next();
        }

        private Token CheckedNext(List<Type> expected)
        {
            if (!expected.Any(PeekItemTypeEquals))
            {
                var typeList = string.Join(" or ", expected);
                ReportError("Expected type " + typeList + " but got " + LexerEnumerator.Peek().Type,
                    LexerEnumerator.Peek());
            }

            // We advance anyway because this possibly catches additional errors
            return LexerEnumerator.Next();
        }
    }
}