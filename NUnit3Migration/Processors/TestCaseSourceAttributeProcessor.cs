using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;
using System.Linq;

namespace NUnit3Migration.Processors
{
    /// <summary>
    ///  The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.
    /// </summary>
    /// <remarks>Would be awesome to recursively find all related (caller) members and make them static also.</remarks>
    public class TestCaseSourceAttributeProcessor : IProcessor
    {
        public void Process(DocumentEditor editor)
        {
            foreach (var node in editor.OriginalRoot.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attribute =>
                    attribute.Name is IdentifierNameSyntax &&
                    ((IdentifierNameSyntax)attribute.Name).Identifier.Text == "TestCaseSource"))
            {
                var testCaseSourceName = node.ArgumentList.Arguments.First().ToString().Replace("\"", "");

                TryAddStatic(editor,
                    editor.OriginalRoot.DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Where(member => member.Identifier.Text == testCaseSourceName)
                        .ToList<dynamic>());
                TryAddStatic(editor,
                    editor.OriginalRoot.DescendantNodes()
                        .OfType<PropertyDeclarationSyntax>()
                        .Where(member => member.Identifier.Text == testCaseSourceName)
                        .ToList<dynamic>());
                TryAddStatic(editor,
                    editor.OriginalRoot.DescendantNodes()
                        .OfType<FieldDeclarationSyntax>()
                        .Where(member => member.Declaration.Variables.Any(v => v.Identifier.ValueText == testCaseSourceName))
                        .ToList<dynamic>());
            }
        }

        private static void TryAddStatic(DocumentEditor editor, List<dynamic> membersFound)
        {
            if (membersFound.Count != 1)
                return;

            var memberDeclaration = membersFound.First();

            if (((IEnumerable<SyntaxToken>)memberDeclaration.Modifiers).Any(token => token.ValueText == "static"))
                return;

            editor.ReplaceNode(
                memberDeclaration,
                memberDeclaration.AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
        }
    }
}
