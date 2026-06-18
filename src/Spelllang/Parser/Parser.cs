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
        private ProgramNode ProgramNode;

        private bool Done = false;

        private TokenEnumerator LexerEnumerator;

        private Dictionary<Type, ParsePrefixExpressionFn> PrefixParserFn;
        private Dictionary<Type, ParseInfixExpressionFn> InfixParserFn;
        private Dictionary<Type, Precedence> PrecedenceMapping;

        private void Setup()
        {
            PrefixParserFn = new Dictionary<Type, ParsePrefixExpressionFn>
            {
                { Type.NUMBER, ParseNumberExpression },
                { Type.STRING, ParseStringExpression },
                { Type.BOOLEAN, ParseBooleanExpression },
                { Type.IDENTIFIER, ParseIdentifierExpression },
                { Type.SEMICOLON, ParseSemicolonExpression },
                { Type.NULL, ParseNullExpression }
            };

            InfixParserFn = new Dictionary<Type, ParseInfixExpressionFn>
            {
                { Type.EQUAL, ParseInfixExpression },
                { Type.PLUS, ParseInfixExpression },
                { Type.MINUS, ParseInfixExpression },
                { Type.MULTIPLY, ParseInfixExpression },
                { Type.DIVIDE, ParseInfixExpression },
                { Type.MODULO, ParseInfixExpression },
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
                { Type.PARENTHESES_LEFT, Precedence.PRECEDENCE_CALL }
            };

            ProgramNode = new ProgramNode();
        }

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

        public ProgramNode GetRootProgram()
        {
            return ProgramNode;
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

        private IStatementNode ParseStatement()
        {
            var result = CurrentItemType() switch
            {
                Type.RETURN => ParseReturnStatement(),
                Type.FUNCTION => ParseFunctionStatement(),
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
                Console.WriteLine("No prefix parser for type " + LexerEnumerator.Current().Type);
                Environment.Exit(1);
                return null;
            }

            var leftExpression = parsePrefixExpressionFn();

            while (!PeekItemTypeEquals(Type.EOF) && precedence < PeekItemPrecedence())
            {
                var infixParser = InfixParserFn.TryGetValue(LexerEnumerator.Peek().Type, out var value) ? value : null;
                if (infixParser == null)
                {
                    Console.WriteLine("No infix parser for type " + LexerEnumerator.Peek().Type);
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

        private IExpressionNode ParseAssignExpression(IExpressionNode identifier)
        {
            var precedence = CurrItemPrecedence();
            LexerEnumerator.Next();

            var value = ParseExpression(precedence);
            if (identifier is IdentifierExpression identifierExpression)
                return new AssignExpression(identifierExpression.IdentifierName, value);
            Console.WriteLine("Expected identifier node but got " + identifier);
            return null;
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
            LexerEnumerator.Next();
            var functionName = FunctionStatement.ANONYMOUS_FUNCTION_NAME;
            if (CurrentItemTypeEquals(Type.IDENTIFIER))
            {
                functionName = LexerEnumerator.Current().Value;
                LexerEnumerator.Next();
            }

            var arguments = ParseExpressionList(Type.PARENTHESES_RIGHT);
            if (arguments.Count > 0 && !arguments.All(item => item != null && item is IdentifierExpression))
            {
                Console.WriteLine("Expected identifiers as arguments but got " + arguments);
                return null;
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

        // TODO: This is clever and dumb...clever because it works, dumb because this has to jump through extra hoops due to my poorly designed API
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
    }
}