using NUnit.Framework;
using NUnit3Migration.Processors;
using System.Threading.Tasks;

namespace NUnit3Migration.Tests.Processors
{
    [TestFixture]
    public class ExpectedExceptionAttributeProcessorTests
    {
        [TestCase(
            @"
            [Test, ExpectedException(typeof (ArgumentException))]
            public void test() {
            int a = 2; }",
            @"
            [Test]
            public void test() {
            Assert.Throws<ArgumentException>(() => { int a = 2; }); }")]
        [TestCase(
            @"
            [Test, ExpectedException()]
            public void test() {
            int a = 2; }",
            @"
            [Test]
            public void test() {
            Assert.Throws<Exception>(() => { int a = 2; }); }")]
        [TestCase(
            @"
            [Test]
            [ExpectedException(typeof(System.NotImplementedException))]
            public void test() {
            int a = 2; }",
            @"
            [Test]
            public void test() {
            Assert.Throws<System.NotImplementedException>(() => { int a = 2; }); }")]
        [TestCase(
            @"
            [Test, ExpectedException(typeof (InvalidOperationException))]
            public void test() {
                using(new Disposable()) {
                    Console.WriteLine(null);
                }
            }",
            @"
            [Test]
            public void test() {
                using(new Disposable()) {
                    Assert.Throws<InvalidOperationException>(() => { Console.WriteLine(null); });
                }
            }")]
        public async Task Process_ExpectedException_IsRemovedAndWholeMethodIsPutInsideBlock(
            string initialCode, string expectedTransformedCode)
        {
            string transformedString = await Program.Process(new[] { new ExpectedExceptionAttributeProcessor() }, initialCode);

            Assert.AreEqual(expectedTransformedCode, transformedString);
        }
    }
}
