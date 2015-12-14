using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace NUnit3Migration.Processors
{
    public class TestCaseAttributeProcessor : IProcessor
    {
        public void Process(DocumentEditor editor)
        {
            var root = editor.OriginalRoot;
            foreach (var node in root.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attribute => ((IdentifierNameSyntax)attribute.Name).Identifier.Text == "TestCase"))
            {
                var resultArgument = node.ArgumentList.Arguments
                    .FirstOrDefault(a => a.NameEquals != null && a.NameEquals.Name.Identifier.Text == "Result");
                if (resultArgument != null)
                {
                    var nameEquals = resultArgument.NameEquals.WithName(SyntaxFactory.IdentifierName("ExpectedResult"));

                    var newNode = resultArgument.WithNameEquals(nameEquals);
                    editor.ReplaceNode(resultArgument, newNode);
                }
            }
        }
    }
}
