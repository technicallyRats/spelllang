using System;
using System.Collections.Generic;
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

    /*
    EMPTY,
    EOF,
    IDENTIFIER,
    PARENTHESES_LEFT,
    PARENTHESES_RIGHT,
    COMMA,
    NUMBER,
    STRING,
    SEMICOLON,
    */
    public class Parser
    {
        ProgramNode ProgramNode;

        bool Done = false;

        TokenEnumerator LexerEnumerator;

        Dictionary<Type, ParsePrefixExpressionFn> PrefixParserFn;
        Dictionary<Type, ParseInfixExpressionFn> InfixParserFn;
        Dictionary<Type, Precedence> PrecedenceMapping;

        private void Setup()
        {
            
            PrefixParserFn = new Dictionary<Type, ParsePrefixExpressionFn>{
                { Type.NUMBER, ParseNumberExpression },
                { Type.STRING, ParseStringExpression },
                { Type.BOOLEAN, ParseBooleanExpression },
                { Type.IDENTIFIER, ParseIdentifierExpression },
                { Type.SEMICOLON, ParseSemicolonExpression}
            };

            InfixParserFn = new Dictionary<Type, ParseInfixExpressionFn>{
                { Type.EQUAL, ParseInfixExpression },
                { Type.ASSIGN, ParseAssignExpression },
                { Type.PARENTHESES_LEFT, ParseCallExpression }
            };

            PrecedenceMapping = new Dictionary<Type, Precedence> {
                { Type.ASSIGN, Precedence.PRECEDENCE_ASSIGN },
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
            List<IStatementNode> statements = new List<IStatementNode>();
            while (!Done && LexerEnumerator.Current().Type != Type.EOF)
            {
                IStatementNode statement = ParseStatement();
                if (statement != null)
                {
                    statements.Add(statement);
                }
            }
            ProgramNode.SetStatements(statements);
        }

        private IStatementNode ParseStatement()
        {
            IStatementNode result = new ExpressionStatement(ParseExpression(Precedence.PRECEDENCE_LOWEST));
            LexerEnumerator.Next();
            return result;
        }

        private IExpressionNode ParseExpression(Precedence precedence)
        {
            ParsePrefixExpressionFn parsePrefixExpressionFn = PrefixParserFn.TryGetValue(LexerEnumerator.Current().Type, out ParsePrefixExpressionFn v )? v : null;
            if (parsePrefixExpressionFn == null)
            {
                Console.WriteLine("No prefix parser for type " + LexerEnumerator.Current().Type);
                Environment.Exit(1);
                return null;
            }

            IExpressionNode leftExpression = parsePrefixExpressionFn();

            while (!PeekItemTypeEquals(Type.EOF) && precedence < PeekItemPrecedence())
            {
                ParseInfixExpressionFn infixParser = InfixParserFn.TryGetValue(LexerEnumerator.Peek().Type, out ParseInfixExpressionFn value)? value : null;
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
            string op = LexerEnumerator.Current().Value;

            Precedence precedence = CurrItemPrecedence();
            LexerEnumerator.Next();

            IExpressionNode right = ParseExpression(precedence);

            return new InfixExpression(left, right, op);
        }

        private IExpressionNode ParseAssignExpression(IExpressionNode identifier)
        {
            Precedence precedence = CurrItemPrecedence();
            LexerEnumerator.Next();

            IExpressionNode value = ParseExpression(precedence);
            if (identifier is IdentifierExpression identifierExpression)
            {
                return new AssignExpression(identifierExpression.IdentifierName, value);
            }
            Console.WriteLine("Expected identifier node but got " + identifier);
            return null;
        }

        private IExpressionNode ParseNumberExpression()
        {
            // Remove _ for syntactic sugar
            String rawValue = LexerEnumerator.Current().Value.Replace("_", "");

            if (rawValue.Contains("."))
            {
                return new FloatExpression(float.Parse(rawValue));
            }
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
            List<IExpressionNode> arguments = ParseExpressionList(Type.PARENTHESES_RIGHT);
            Console.WriteLine(new CallExpression(left, arguments).ToReadableString());
            return new CallExpression(left, arguments);
        }

        private List<IExpressionNode> ParseExpressionList(Type terminatingType)
        {
            List<IExpressionNode> result = new List<IExpressionNode>();

            if (PeekItemTypeEquals(terminatingType))
            {
                return result;
            }
            LexerEnumerator.Next();

            while (!CurrentItemTypeEquals(terminatingType))
            {
                result.Add(ParseExpression(Precedence.PRECEDENCE_LOWEST));
                LexerEnumerator.Next();
                if (CurrentItemTypeEquals(Type.COMMA))
                {
                    LexerEnumerator.Next();
                }
            }

            return result;
        }

        private IExpressionNode ParseSemicolonExpression()
        {
            return new SemicolonExpression();
        }

        private Precedence CurrItemPrecedence()
        {
            return PrecedenceMapping.TryGetValue(LexerEnumerator.Current().Type, out Precedence value )? value : Precedence.PRECEDENCE_LOWEST;
        }

        private Precedence PeekItemPrecedence()
        {
            return PrecedenceMapping.TryGetValue(LexerEnumerator.Peek().Type, out Precedence value )? value : Precedence.PRECEDENCE_LOWEST;
        }

        private bool PeekItemTypeEquals(Type expected)
        {
            return LexerEnumerator.Peek().Type == expected;
        }

        private bool CurrentItemTypeEquals(Type expected)
        {
            return LexerEnumerator.Current().Type == expected;
        }
    }
}
