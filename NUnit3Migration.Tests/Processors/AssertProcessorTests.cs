using NUnit.Framework;
using NUnit3Migration.Processors;
using System.Threading.Tasks;

namespace NUnit3Migration.Tests.Processors
{
    internal class AssertProcessorTests
    {
        [TestCase(
            @"
            Assert.IsNullOrEmpty(234235);",
            @"
            Assert.That(234235, Is.Null.Or.Empty);")]
        [TestCase(
            @"Assert.IsNullOrEmpty(setup.StructureUnderTest.Content.Text);",
            @"Assert.That(setup.StructureUnderTest.Content.Text, Is.Null.Or.Empty);")]
        [TestCase(
            @"Assert.IsNotNullOrEmpty(setup.StructureUnderTest.Content.Text);",
            @"Assert.That(setup.StructureUnderTest.Content.Text, Is.Not.Null.Or.Empty);")]
        public async Task Process_NullOrEmptyAssert_IsTransformedToAssertThat(
            string initialCode, string expectedTransformedCode)
        {
            string transformedString = await Program.Process(
                new[] { new AssertProcessor() }, WrapInMethod(initialCode));

            Assert.AreEqual(WrapInMethod(expectedTransformedCode), transformedString);
        }

        private string WrapInMethod(string contents)
        {
            return $"public void method() {{ {contents} }}";
        }
    }
}
