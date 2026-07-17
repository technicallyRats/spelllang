using NUnit.Framework;
using Spelllang.Parser;

namespace Spelllang.Tests.Parser
{
    public class ErrorTests
    {
        [Test]
        public void RenderError()
        {
            var err = new ParsingError(5, 5, "This failed");
            var sourceCode = "This this? trailing end";
            var actual = err.Show(sourceCode);
            var expected = @"This failed
This this? trai
     ^^^^^     ";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void RenderError_LeftLimited()
        {
            var err = new ParsingError(0, 5, "This failed");
            var sourceCode = "This this? trailing end";
            var actual = err.Show(sourceCode);
            var expected = @"This failed
This this?
^^^^^     ";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void RenderError_RightLimited()
        {
            var err = new ParsingError(5, 5, "This failed");
            var sourceCode = "This this?";
            var actual = err.Show(sourceCode);
            var expected = @"This failed
This this?
     ^^^^^";
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}