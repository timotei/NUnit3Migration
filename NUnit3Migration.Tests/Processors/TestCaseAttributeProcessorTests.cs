using NUnit.Framework;
using NUnit3Migration.Processors;
using System.Threading.Tasks;

namespace NUnit3Migration.Tests.Processors
{
    internal class TestCaseAttributeProcessorTests
    {
        [TestCase(
            @"[TestCase("""", false, Result= """")]
            [TestCase(""2"", false, Result = ""2"")]
            public string DoTest(string parameter, bool other) {}",
            @"[TestCase("""", false, ExpectedResult= """")]
            [TestCase(""2"", false, ExpectedResult = ""2"")]
            public string DoTest(string parameter, bool other) {}")]

        [TestCase(
            @"[System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [System.CodeDom.Compiler.GeneratedCodeAttribute(""PresentationBuildTasks"", ""4.0.0.0"")]
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            public sealed class GeneratedInternalTypeHelper : System.Windows.Markup.InternalTypeHelper{}",
            @"[System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [System.CodeDom.Compiler.GeneratedCodeAttribute(""PresentationBuildTasks"", ""4.0.0.0"")]
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            public sealed class GeneratedInternalTypeHelper : System.Windows.Markup.InternalTypeHelper{}")]

        public async Task Process_ResultParameter_IsTransformedToExpectedResult(
            string initialCode, string expectedTransformedCode)
        {
            string transformedString = await Program.Process(new[] { new TestCaseAttributeProcessor() }, initialCode);

            Assert.AreEqual(expectedTransformedCode, transformedString);
        }
    }
}
