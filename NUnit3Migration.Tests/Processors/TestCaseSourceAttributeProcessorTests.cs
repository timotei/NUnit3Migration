using NUnit.Framework;
using NUnit3Migration.Processors;
using System.Threading.Tasks;

namespace NUnit3Migration.Tests.Processors
{
    internal class TestCaseSourceAttributeProcessorTests
    {
        [TestCase(
            @"
            public IEnumerable GetTestCases() { return null; }
            [TestCaseSource(""GetTestCases"")]
            public string DoTest() {}",
            @"
            public static IEnumerable GetTestCases() { return null; }
            [TestCaseSource(""GetTestCases"")]
            public string DoTest() {}")]
        [TestCase(
            @"
            public IEnumerable TheTestCases;
            [TestCaseSource(""TheTestCases"")]
            public string DoTest() {}",
            @"
            public static IEnumerable TheTestCases;
            [TestCaseSource(""TheTestCases"")]
            public string DoTest() {}")]
        [TestCase(
            @"
            public static IEnumerable TheTestCases;
            [TestCaseSource(""TheTestCases"")]
            public string DoTest() {}",
            @"
            public static IEnumerable TheTestCases;
            [TestCaseSource(""TheTestCases"")]
            public string DoTest() {}")]
        [TestCase(
            @"
            public IEnumerable GetTestCases { get { return null; } }
            [TestCaseSource(""GetTestCases"")]
            public string DoTest() {}",
            @"
            public static IEnumerable GetTestCases { get { return null; } }
            [TestCaseSource(""GetTestCases"")]
            public string DoTest() {}")]
        public async Task Process_TestCaseSourceString_AssociatedElementIsConvertedToStatic(
            string initialCode, string expectedTransformedCode)
        {
            string transformedString = await Program.Process(new[] { new TestCaseSourceAttributeProcessor() }, initialCode);

            Assert.AreEqual(expectedTransformedCode, transformedString);
        }
    }
}
