using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace NUnit3Migration.Processors
{
    public class IgnoreAttributeProcessor : IProcessor
    {
        public void Process(DocumentEditor editor)
        {
            foreach (var node in editor.OriginalRoot.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attribute =>
                    attribute.Name is IdentifierNameSyntax &&
                    ((IdentifierNameSyntax)attribute.Name).Identifier.Text == "Ignore"))
            {
                if (node.ArgumentList == null ||
                    node.ArgumentList.Arguments.Count == 0)
                {
                    //var argumentList = SyntaxFactory.AttributeArgumentList(
                    //    SyntaxFactory.SingletonSeparatedList(
                    //        SyntaxFactory.AttributeArgument(
                    //            SyntaxFactory.LiteralExpression(
                    //                SyntaxKind.StringLiteralExpression,
                    //                SyntaxFactory.Literal(
                    //                    SyntaxFactory.TriviaList(),
                    //                    @"asd",
                    //                    @"""asd""",
                    //                    SyntaxFactory.TriviaList())))));

                   // editor.ReplaceNode(node, node.WithName(SyntaxFactory.IdentifierName("asd")));
                }
            }
        }
    }
}
