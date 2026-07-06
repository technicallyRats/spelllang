using System.Collections.Generic;
using System.Text;

namespace Spelllang.AST
{
    public interface IAstNode
    {
        string ToReadableString();
    }

    public interface IExpressionNode : IAstNode
    {
    }

    public interface IStatementNode : IAstNode
    {
    }

    public struct ProgramNode : IAstNode
    {
        public List<IStatementNode> Statements { get; set; }

        public ProgramNode(List<IStatementNode> statements)
        {
            Statements = statements;
        }

        public void SetStatements(List<IStatementNode> statements)
        {
            Statements = statements;
        }

        public string ToReadableString()
        {
            // Start with 64 expected size, this can outgrow therefore this is not critical
            var sb = new StringBuilder("Statements:", 64);
            sb.AppendLine();
            var counter = 1;
            if (Statements != null)
                Statements.ForEach(element => { sb.AppendLine(counter++ + ".\t" + element.ToReadableString()); });
            return sb.ToString();
        }

        public bool IsEmpty()
        {
            if (Statements == null) return true;
            return Statements.Count == 0;
        }
    }

    public struct FunctionStatement : IStatementNode
    {
        public static string ANONYMOUS_FUNCTION_NAME = "anon";
        public string FunctionName { get; }
        public List<IdentifierExpression> ArgumentIdentifiers { get; }

        public ProgramNode Program;

        public FunctionStatement(string functionName, List<IdentifierExpression> argumentIdentifiers,
            ProgramNode program)
        {
            FunctionName = functionName;
            ArgumentIdentifiers = argumentIdentifiers;
            Program = program;
        }

        public string ToReadableString()
        {
            {
                // Start with 64 expected size, this can outgrow therefore this is not critical
                var sb = new StringBuilder("Function ", 64);
                sb.Append(FunctionName);
                sb.Append("(");
                ArgumentIdentifiers.ForEach(element =>
                {
                    sb.Append(element.ToReadableString());
                    sb.Append(", ");
                });
                sb.Append(")");
                sb.Append(":");
                sb.AppendLine();
                sb.Append(Program.ToReadableString());
                return sb.ToString();
            }
        }

        public bool isAnonymous()
        {
            return FunctionName == ANONYMOUS_FUNCTION_NAME;
        }
    }

    public struct ReturnStatement : IStatementNode
    {
        public IExpressionNode ReturnValue;

        public ReturnStatement(IExpressionNode returnValue)
        {
            ReturnValue = returnValue;
        }

        public string ToReadableString()
        {
            return $"Returning {ReturnValue.ToReadableString()}";
        }
    }

    public struct IfStatement : IStatementNode
    {
        public ProgramNode Primary;
        public ProgramNode Secondary;
        public IExpressionNode Condition;

        public IfStatement(ProgramNode primary, ProgramNode secondary, IExpressionNode condition)
        {
            Primary = primary;
            Secondary = secondary;
            Condition = condition;
        }

        public string ToReadableString()
        {
            return
                $"If statement with condition {Condition.ToReadableString()}\nPrimary: {Primary.ToReadableString()}\nSecondary: {Secondary.ToReadableString()}";
        }

        public bool HasSecondary()
        {
            return !Secondary.IsEmpty();
        }
    }

    public struct WhileStatement : IStatementNode
    {
        public ProgramNode Body;
        public IExpressionNode Condition;

        public WhileStatement(ProgramNode body, IExpressionNode condition)
        {
            Body = body;
            Condition = condition;
        }

        public string ToReadableString()
        {
            return
                $"While statement with condition {Condition.ToReadableString()}\nBody: {Body.ToReadableString()}";
        }
    }

    public struct BreakStatement : IStatementNode
    {
        public string ToReadableString()
        {
            return $"Break!";
        }
    }

    public struct PrefixExpression : IExpressionNode
    {
        public string Operator { get; }
        public IExpressionNode Right { get; }

        public PrefixExpression(string op, IExpressionNode right)
        {
            Operator = op;
            Right = right;
        }

        public string ToReadableString()
        {
            return $"Prefix {Operator} for {Right.ToReadableString()}";
        }
    }

    public struct InfixExpression : IExpressionNode
    {
        public string Operator { get; }
        public IExpressionNode Left { get; }
        public IExpressionNode Right { get; }

        public InfixExpression(IExpressionNode left, IExpressionNode right, string op)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public string ToReadableString()
        {
            return $"Infix {Left.ToReadableString()} {Operator} {Right.ToReadableString()}";
        }
    }

    // TODO: I now have a statement and an expression for this...I think it should be the first but thsi needs to be deduplicated
    public struct AssignExpression : IExpressionNode
    {
        public string Identifier { get; }
        public IExpressionNode Value { get; }

        public AssignExpression(string identifier, IExpressionNode value)
        {
            Identifier = identifier;
            Value = value;
        }

        public string ToReadableString()
        {
            return $"Assigning {Value.ToReadableString()} to {Identifier}";
        }
    }

    public struct IntegerExpression : IExpressionNode
    {
        public int Value { get; }

        public IntegerExpression(int value)
        {
            Value = value;
        }

        public string ToReadableString()
        {
            return $"Integer {Value}";
        }
    }

    public struct BooleanExpression : IExpressionNode
    {
        public bool Value { get; }

        public BooleanExpression(bool value)
        {
            Value = value;
        }

        public string ToReadableString()
        {
            return $"Boolean {Value}";
        }
    }

    // TODO: do we need automatic conversion to double?
    public struct FloatExpression : IExpressionNode
    {
        public float Value { get; }

        public FloatExpression(float value)
        {
            Value = value;
        }

        public string ToReadableString()
        {
            return $"Floating Point {Value}";
        }
    }

    public struct StringExpression : IExpressionNode
    {
        public string Value { get; }

        public StringExpression(string value)
        {
            Value = value;
        }

        public string ToReadableString()
        {
            return $"String {Value}";
        }
    }

    public struct IdentifierExpression : IExpressionNode
    {
        public string IdentifierName { get; }

        public IdentifierExpression(string identifierName)
        {
            IdentifierName = identifierName;
        }

        public string ToReadableString()
        {
            return $"Identifier {IdentifierName}";
        }
    }

    public struct NullExpression : IExpressionNode
    {
        public string ToReadableString()
        {
            return $"Null";
        }
    }

    //TODO : I am not sure if this is smart
    public struct SemicolonExpression : IExpressionNode
    {
        public string ToReadableString()
        {
            return "Semicolon";
        }
    }

    public struct CallExpression : IExpressionNode
    {
        public IExpressionNode Identifier { get; }
        public List<IExpressionNode> Arguments { get; }

        public CallExpression(IExpressionNode identifier, List<IExpressionNode> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        public string ToReadableString()
        {
            // Start with 64 expected size, this can outgrow therefore this is not critical
            var sb = new StringBuilder("Calling ", 64);
            if (Arguments.Count == 0)
            {
                sb.AppendFormat("{0} with no arguments ", Identifier.ToReadableString());
                return sb.ToString();
            }

            sb.AppendFormat("{0} with arguments ", Identifier.ToReadableString());
            Arguments.ForEach(element => { sb.AppendFormat("{0},", element.ToReadableString()); });
            // This feels illegal
            sb.Length--;
            return sb.ToString();
        }
    }

    public struct ExpressionStatement : IStatementNode
    {
        public IExpressionNode Expression { get; }

        public ExpressionStatement(IExpressionNode expression)
        {
            Expression = expression;
        }

        public string ToReadableString()
        {
            return $"Expression {Expression.ToReadableString()}";
        }
    }

    public struct AssignStatement : IStatementNode
    {
        //public IExpressionNode Identifier { get; }
        public string Identifier { get; }
        public IExpressionNode Value { get; }

        public AssignStatement(string identifier, IExpressionNode value)
        {
            Identifier = identifier;
            Value = value;
        }

        public string ToReadableString()
        {
            return $"Assign {Value.ToReadableString()} to {Identifier}";
        }
    }
}