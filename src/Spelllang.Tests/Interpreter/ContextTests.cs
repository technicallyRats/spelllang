using NUnit.Framework;
using Spelllang.Interpreter;

namespace Spelllang.Tests.Interpreter
{
    [TestFixture]
    public class ContextTests
    {
        private System.IO.TextWriter? _originalOut;

        [SetUp]
        public void SetUp()
        {
            _originalOut = System.Console.Out;
            System.Console.SetOut(new System.IO.StringWriter());
        }

        [TearDown]
        public void TearDown()
        {
            System.Console.SetOut(_originalOut!);
        }

        [Test]
        public void Register_And_Retrieve_ReturnsSameValue()
        {
            var ctx = new Context(null);
            ctx.Register("x", new RuntimeInt(42));

            var result = ctx.Retrieve("x");

            InterpreterTestUtils.AssertRuntimeInt(result, 42);
        }

        [Test]
        public void Retrieve_MissingKey_ReturnsNull()
        {
            var ctx = new Context(null);

            var result = ctx.Retrieve("nonexistent");

            Assert.That(result, Is.Null);
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

            Assert.That(ctx.Retrieve("x"), Is.Null);
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

            Assert.That(child1.Retrieve("b"), Is.Null);
            Assert.That(child2.Retrieve("a"), Is.Null);
            InterpreterTestUtils.AssertRuntimeInt(child1.Retrieve("x"), 0);
            InterpreterTestUtils.AssertRuntimeInt(child2.Retrieve("x"), 0);
        }

        [Test]
        public void Context_RegistersBuiltinsOnConstruction()
        {
            var builtin = new TestBuiltins.PrintCapturingBuiltin();
            var builtins = new System.Collections.Generic.List<(string name, Spelllang.Builtins.IRuntimeBuiltin value)>
            {
                ("PRINT", builtin)
            };

            var ctx = new Context(null, builtins);

            Assert.That(ctx.Retrieve("PRINT"), Is.InstanceOf<TestBuiltins.PrintCapturingBuiltin>());
        }

        [Test]
        public void Context_WithNullEnclosing_IsRoot()
        {
            var ctx = new Context(null);

            Assert.That(ctx.Retrieve("anything"), Is.Null);
        }
    }
}
