using System.Collections.Generic;
using NUnit.Framework;
using Spelllang.Builtins;
using Spelllang.Interpreter.Importer;

namespace Spelllang.Tests.Interpreter
{
    public class ImportTests
    {
        [Test]
        public void Named_Import()
        {
            var providedLibrary = new Dictionary<string, string>
            {
                { "math", "function add(a,b) {return a + b}" }
            };
            var resolvers = new List<IImportResolver> { new InMemoryImportResolver(providedLibrary) };
            var interpreter = InterpreterTestUtils.BuildInterpreter(@"
import math as math;
math.add(1,2);
",
                new List<(string name, IRuntimeBuiltin value)>(), resolvers);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 3);
        }

        [Test]
        public void Import()
        {
            var providedLibrary = new Dictionary<string, string>
            {
                { "math", "function add(a,b) {return a + b}" }
            };
            var resolvers = new List<IImportResolver> { new InMemoryImportResolver(providedLibrary) };
            var interpreter = InterpreterTestUtils.BuildInterpreter(@"
import math;
add(1,2);
",
                new List<(string name, IRuntimeBuiltin value)>(), resolvers);

            var result = interpreter.Run();

            InterpreterTestUtils.AssertRuntimeInt(result, 3);
        }
    }
}