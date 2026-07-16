using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;
using Spelllang.Tests.TestUtils;

namespace Spelllang.Tests.Parser
{
    public class ValueParsingTests
    {
        public static IEnumerable NumberTestCases
        {
            get
            {
                var inc = new Incrementer();
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "1", inc.Increment("1")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new IntegerExpression(1)
                        )
                    })
                ).SetName("Simple Int");
                inc.Reset();
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "1_000", inc.Increment("1_000")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new IntegerExpression(1000)
                        )
                    })
                ).SetName("Formatted Int");
                inc.Reset();
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "3.14", inc.Increment("3.14")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new FloatExpression(3.14f)
                        )
                    })
                ).SetName("Simple Float");
                inc.Reset();
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "3_141.59265359", inc.Increment("3_141.59265359")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new FloatExpression(3141.59265359f)
                        )
                    })
                ).SetName("Formatted Float");
            }
        }

        public static IEnumerable BoolTestCases
        {
            get
            {
                var inc = new Incrementer();
                yield return new TestCaseData(
                    new List<Token> { new(Type.BOOLEAN, "true", inc.Increment("true")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new BooleanExpression(true)
                        )
                    })
                ).SetName("True");
                inc.Reset();
                yield return new TestCaseData(
                    new List<Token> { new(Type.BOOLEAN, "false", inc.Increment("false")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new BooleanExpression(false)
                        )
                    })
                ).SetName("False");
            }
        }

        public static IEnumerable StringTestCases
        {
            get
            {
                var inc = new Incrementer();
                yield return new TestCaseData(
                    new List<Token> { new(Type.STRING, "hello world", inc.Increment("hello world")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new StringExpression("hello world")
                        )
                    })
                ).SetName("True");
            }
        }

        public static IEnumerable NullTestCases
        {
            get
            {
                var inc = new Incrementer();
                yield return new TestCaseData(
                    new List<Token> { new(Type.NULL, "null", inc.Increment("null")) },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new NullExpression()
                        )
                    })
                ).SetName("Null");
            }
        }

        [Test]
        [TestCaseSource(nameof(NumberTestCases))]
        public void Parse_Numbers(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(BoolTestCases))]
        public void Parse_Bool(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(StringTestCases))]
        public void Parse_String(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }

        [Test]
        [TestCaseSource(nameof(NullTestCases))]
        public void Parse_Null(List<Token> input, ProgramNode expected)
        {
            ParsingTestUtils.AssertAst(ParsingTestUtils.Parse(input), expected);
        }
    }
}