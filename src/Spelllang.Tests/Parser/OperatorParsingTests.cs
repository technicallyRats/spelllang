using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Parser
{
    [TestFixture]
    public class OperatorParsingTests
    {
        public static IEnumerable SimpleInfixTestCases
        {
            get
            {
                yield return BuildSimpleInfixTestCase("+", Type.PLUS).SetName("Simple plus");
                yield return BuildSimpleInfixTestCase("-", Type.MINUS).SetName("Simple minus");
                yield return BuildSimpleInfixTestCase("*", Type.MULTIPLY).SetName("Simple multiply");
                yield return BuildSimpleInfixTestCase("/", Type.DIVIDE).SetName("Simple divide");
                yield return BuildSimpleInfixTestCase("%", Type.MODULO).SetName("Simple modulo");
            }
        }

        public static IEnumerable SimplePrefixTestCases
        {
            get
            {
                yield return BuildSimpleIntegerPrefixTestCase("+", Type.PLUS).SetName("Plus prefix");
                yield return BuildSimpleIntegerPrefixTestCase("-", Type.MINUS).SetName("Plus prefix");
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NOT, "!"),
                        new(Type.BOOLEAN, "true")
                    },
                    new ProgramNode(new List<IStatementNode>
                        {
                            new ExpressionStatement(new PrefixExpression(
                                    "!",
                                    new BooleanExpression(true)
                                )
                            )
                        }
                    )
                ).SetName("Not prefix");
            }
        }

        [Test]
        [TestCaseSource(nameof(SimplePrefixTestCases))]
        public void Parse_PrefixInfix(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        private static TestCaseData BuildSimpleInfixTestCase(string op, Type operatorType)
        {
            return new TestCaseData(
                new List<Token>
                {
                    new(Type.NUMBER, "1"),
                    new(operatorType, op),
                    new(Type.NUMBER, "1")
                },
                new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(new InfixExpression(
                                new IntegerExpression(1),
                                new IntegerExpression(1),
                                op
                            )
                        )
                    }
                )
            );
        }

        private static TestCaseData BuildSimpleIntegerPrefixTestCase(string op, Type operatorType)
        {
            return new TestCaseData(
                new List<Token>
                {
                    new(operatorType, op),
                    new(Type.NUMBER, "1")
                },
                new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(new PrefixExpression(
                                op,
                                new IntegerExpression(1)
                            )
                        )
                    }
                )
            );
        }
    }
}