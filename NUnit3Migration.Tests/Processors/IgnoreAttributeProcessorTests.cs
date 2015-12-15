using NUnit.Framework;
using NUnit3Migration.Processors;
using System.Threading.Tasks;

namespace NUnit3Migration.Tests.Processors
{
    internal class IgnoreAttributeProcessorTests
    {
        [TestCase(
            @"[Ignore]
            public string DoTest(string parameter, bool other) {}",
            @"[Ignore(""No reason"")]
            public string DoTest(string parameter, bool other) {}")]
        [TestCase(
            @"[Ignore()]
            public string DoTest(string parameter, bool other) {}",
            @"[Ignore(""No reason"")]
            public string DoTest(string parameter, bool other) {}")]
        public async Task Process_IgnoreAttributeWithoutReason_AddsPlaceholderText(
            string initialCode, string expectedTransformedCode)
        {
            string transformedString = await Program.Process(new[] { new IgnoreAttributeProcessor() }, initialCode);

            Assert.AreEqual(expectedTransformedCode, transformedString);
        }
    }
}
