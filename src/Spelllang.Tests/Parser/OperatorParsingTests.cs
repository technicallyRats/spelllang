using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;

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
                var inc = new Incrementer();
                yield return new TestCaseData(
                    new List<Token>
                    {
                        new(Type.NOT, "!", inc.Increment("!")),
                        new(Type.BOOLEAN, "true", inc.Increment("true"))
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
            ParsingTestUtils.AssertParserState(ParsingTestUtils.Parse(input), expected);
        }

        private static TestCaseData BuildSimpleInfixTestCase(string op, Type operatorType)
        {
            var inc = new Incrementer();
            return new TestCaseData(
                new List<Token>
                {
                    new(Type.NUMBER, "1", inc.Increment("1")),
                    new(operatorType, op, inc.Increment(op)),
                    new(Type.NUMBER, "1", inc.Increment("1"))
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
            var inc = new Incrementer();
            return new TestCaseData(
                new List<Token>
                {
                    new(operatorType, op, inc.Increment(op)),
                    new(Type.NUMBER, "1", inc.Increment("1"))
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