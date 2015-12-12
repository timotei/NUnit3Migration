using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace NUnit3Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"F:\src\ullink\git\ul-trader-extension\" +
                          "desk/extension/test/legacy/ultrader.test/OrderEntryTests/ULOrderEntryFormBooksTests.cs";
            SyntaxTree syntaxTree = null;
            using (var reader = new StreamReader(path))
            {
                syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd());
            }
            var root = syntaxTree.GetRoot();
            //foreach (var node in root.DescendantNodes()
            //    .OfType<AttributeSyntax>()
            //    .Where(attribute => ((IdentifierNameSyntax)attribute.Name).Identifier.Text == "TestCase")
            //    )
            //{
            //    var resultArgument = node.ArgumentList.Arguments
            //        .FirstOrDefault(a => a.NameEquals != null && a.NameEquals.Name.Identifier.Text == "Result");
            //    if (resultArgument != null)
            //    {
            //        var nameEquals = resultArgument.NameEquals.WithName(SyntaxFactory.IdentifierName("ExpectedResult"));

            //        //root = root.ReplaceNode(resultArgument, resultArgument);//.WithNameEquals(nameEquals));
            //        break;
            //    }
            //    Console.WriteLine(node);
            //}
            
            new StreamWriter(path).Write(root.ToFullString());
        }
    }
}
