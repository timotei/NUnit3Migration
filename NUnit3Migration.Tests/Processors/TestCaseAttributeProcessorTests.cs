using NUnit.Framework;
using NUnit3Migration.Processors;
using System.Threading.Tasks;

namespace NUnit3Migration.Tests.Processors
{
    internal class TestCaseAttributeProcessorTests
    {
        [TestCase(@"
                [TestCase("""", false, Result= """")]
                [TestCase(""2"", false, Result= ""2"")]
                public string DoTest(string parameter, bool other)
                {
                    //Arrange
                    //Act
                    //Assert
                    Assert.IsTrue(true);
                }",
             @"
                [TestCase("""", false, ExpectedResult= """")]
                [TestCase(""2"", false, ExpectedResult= ""2"")]
                public string DoTest(string parameter, bool other)
                {
                    //Arrange
                    //Act
                    //Assert
                    Assert.IsTrue(true);
                }")]
        public async Task Process_ResultParameter_IsTransformedToExpectedResult(
            string initialCode, string expectedTransformedCode)
        {
            string transformedString = await Program.Process(new[] { new TestCaseAttributeProcessor() }, initialCode);

            Assert.AreEqual(expectedTransformedCode, transformedString);
        }
    }
}
