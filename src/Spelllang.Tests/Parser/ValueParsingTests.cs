using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.AST;
using Spelllang.Lexer;

namespace Spelllang.Tests.Parser
{
    public class ValueParsingTests
    {
        public static IEnumerable NumberTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "1") },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new IntegerExpression(1)
                        )
                    })
                ).SetName("Simple Int");
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "1_000") },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new IntegerExpression(1000)
                        )
                    })
                ).SetName("Formatted Int");
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "3.14") },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new FloatExpression(3.14f)
                        )
                    })
                ).SetName("Simple Float");
                yield return new TestCaseData(
                    new List<Token> { new(Type.NUMBER, "3_141.59265359") },
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
                yield return new TestCaseData(
                    new List<Token> { new(Type.BOOLEAN, "true") },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new BooleanExpression(true)
                        )
                    })
                ).SetName("True");
                yield return new TestCaseData(
                    new List<Token> { new(Type.BOOLEAN, "false") },
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
                yield return new TestCaseData(
                    new List<Token> { new(Type.STRING, "hello world") },
                    new ProgramNode(new List<IStatementNode>
                    {
                        new ExpressionStatement(
                            new StringExpression("hello world")
                        )
                    })
                ).SetName("True");
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
    }
}