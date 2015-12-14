using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace NUnit3Migration.Processors
{
    public class AssertProcessor : IProcessor
    {
        public void Process(DocumentEditor editor)
        {
            foreach (var node in editor.OriginalRoot.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var expressionString = node.Expression.ToString();

                if (expressionString == "Assert.IsNullOrEmpty" ||
                    expressionString == "Assert.IsNotNullOrEmpty")
                {
                    var expression = SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(
                            SyntaxFactory.Identifier(
                                node.Expression.GetLeadingTrivia(), "Assert", SyntaxTriviaList.Empty)),
                        SyntaxFactory.IdentifierName("That"));

                    var argumentList = SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                node.ArgumentList.Arguments.First(),
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.CommaToken,
                                    SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                GetAssertion(expressionString),
                                                SyntaxFactory.IdentifierName("Null"))
                                                .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken)),
                                            SyntaxFactory.IdentifierName("Or"))
                                            .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken)),
                                        SyntaxFactory.IdentifierName("Empty"))
                                        .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken)))
                            }));

                    editor.ReplaceNode(node, node.WithExpression(expression).WithArgumentList(argumentList));
                }
            }
        }

        private ExpressionSyntax GetAssertion(string expressionString)
        {
            if (!expressionString.EndsWith("IsNotNullOrEmpty"))
                return SyntaxFactory.IdentifierName("Is");

            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("Is"),
                SyntaxFactory.IdentifierName("Not"))
                .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken));
        }
    }
}
