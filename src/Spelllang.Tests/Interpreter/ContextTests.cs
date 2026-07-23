using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Spelllang.Builtins;
using Spelllang.Interpreter;
using Spelllang.Tests.TestBuiltins;

namespace Spelllang.Tests.Interpreter
{
    [TestFixture]
    public class ContextTests
    {
        [SetUp]
        public void SetUp()
        {
            _originalOut = Console.Out;
            Console.SetOut(new StringWriter());
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut!);
        }

        private TextWriter? _originalOut;

        [Test]
        public void Register_And_Retrieve_ReturnsSameValue()
        {
            var ctx = new Context(null);
            ctx.Register("x", new Context(null, new RuntimeInt(42)));

            var result = ctx.Retrieve("x");

            InterpreterTestUtils.AssertRuntimeInt(result, 42);
        }

        [Test]
        public void Retrieve_MissingKey_ReturnsNull()
        {
            var ctx = new Context(null);

            InterpreterTestUtils.AssertRuntimeError(ctx.Retrieve("nonexistent"),
                "Tried retrieving unknown variable nonexistent (nonexistent)");
        }

        [Test]
        public void Register_OverwritesExistingKey()
        {
            var ctx = new Context(null);
            ctx.Register("x", new RuntimeInt(1));
            ctx.Register("x", new RuntimeInt(2));

            var result = ctx.Retrieve("x");

            InterpreterTestUtils.AssertRuntimeInt(result, 2);
        }

        [Test]
        public void Retrieve_WalksEnclosingChain()
        {
            var outer = new Context(null);
            outer.Register("x", new RuntimeString("outer"));

            var inner = new Context(outer);

            Assert.That(inner.Retrieve("x"), Is.Not.Null);

            var result = inner.Retrieve("x");
            InterpreterTestUtils.AssertRuntimeString(result, "outer");
        }

        [Test]
        public void InnerScope_Shadows_OuterScope()
        {
            var outer = new Context(null);
            outer.Register("x", new RuntimeInt(1));

            var inner = new Context(outer);
            inner.Register("x", new RuntimeInt(2));

            InterpreterTestUtils.AssertRuntimeInt(inner.Retrieve("x"), 2);
            InterpreterTestUtils.AssertRuntimeInt(outer.Retrieve("x"), 1);
        }

        [Test]
        public void Evict_RemovesFromLocalScope()
        {
            var ctx = new Context(null);
            ctx.Register("x", new RuntimeInt(1));
            ctx.Evict("x");

            InterpreterTestUtils.AssertRuntimeError(ctx.Retrieve("x"),
                "Tried retrieving unknown variable x (x)");
        }

        [Test]
        public void Evict_DoesNotAffectEnclosingScope()
        {
            var outer = new Context(null);
            outer.Register("x", new RuntimeInt(1));

            var inner = new Context(outer);
            inner.Evict("x");

            InterpreterTestUtils.AssertRuntimeInt(outer.Retrieve("x"), 1);
        }

        [Test]
        public void Enclose_CreatesChildWithAccessToParent()
        {
            var outer = new Context(null);
            outer.Register("x", new RuntimeInt(10));

            var inner = outer.Enclose();

            InterpreterTestUtils.AssertRuntimeInt(inner.Retrieve("x"), 10);
        }

        [Test]
        public void SiblingScopes_DoNotShareVariables()
        {
            var root = new Context(null);
            root.Register("x", new RuntimeInt(0));

            var child1 = root.Enclose();
            child1.Register("a", new RuntimeInt(1));

            var child2 = root.Enclose();
            child2.Register("b", new RuntimeInt(2));

            InterpreterTestUtils.AssertRuntimeError(child1.Retrieve("b"),
                "Tried retrieving unknown variable b (b)");
            InterpreterTestUtils.AssertRuntimeError(child2.Retrieve("a"),
                "Tried retrieving unknown variable a (a)");
            InterpreterTestUtils.AssertRuntimeInt(child1.Retrieve("x"), 0);
            InterpreterTestUtils.AssertRuntimeInt(child2.Retrieve("x"), 0);
        }

        [Test]
        public void Context_RegistersBuiltinsOnConstruction()
        {
            var builtin = new PrintCapturingBuiltin();
            var builtins = new List<(string name, IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };

            var ctx = new Context(null, builtins);

            Assert.That(ctx.Retrieve("PRINT"), Is.InstanceOf<PrintCapturingBuiltin>());
        }

        [Test]
        public void Context_WithNullEnclosing_IsRoot()
        {
            var ctx = new Context(null);

            InterpreterTestUtils.AssertRuntimeError(ctx.Retrieve("anything"),
                "Tried retrieving unknown variable anything (anything)");
        }
    }
}