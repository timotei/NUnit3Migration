using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace NUnit3Migration.Processors
{
    public class ExpectedExceptionAttributeProcessor : IProcessor
    {
        public void Process(DocumentEditor editor)
        {
            foreach (var node in editor.OriginalRoot.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attribute =>
                    attribute.Name is IdentifierNameSyntax &&
                    ((IdentifierNameSyntax)attribute.Name).Identifier.Text == "ExpectedException"))
            {
                var typeofExpression = (TypeOfExpressionSyntax)node.ArgumentList.Arguments.FirstOrDefault()?.Expression;
                var expectedExceptionType = (NameSyntax)typeofExpression?.Type;

                if (expectedExceptionType == null)
                {
                    expectedExceptionType = SyntaxFactory.IdentifierName("Exception");
                }

                var parentMethod = (MethodDeclarationSyntax)node.Parent.Parent;
                var lastStatement = parentMethod.Body.DescendantNodes().OfType<StatementSyntax>().Last();

                var assertThrowsStatement =
                    GetAssertThrows(lastStatement.GetLeadingTrivia(), expectedExceptionType)
                    .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken));
                var expression = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(assertThrowsStatement)
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.ParenthesizedLambdaExpression(
                                            SyntaxFactory.Block(
                                                SyntaxFactory.SingletonList(
                                                    lastStatement
                                                        .WithLeadingTrivia(SyntaxTriviaList.Empty)
                                                        .WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Space))))
                                                .WithOpenBraceToken(
                                                    SyntaxFactory.Token(
                                                        SyntaxFactory.TriviaList(),
                                                        SyntaxKind.OpenBraceToken,
                                                        SyntaxFactory.TriviaList(SyntaxFactory.Space)))
                                                .WithCloseBraceToken(
                                                    SyntaxFactory.Token(SyntaxKind.CloseBraceToken)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList()
                                                    .WithOpenParenToken(
                                                        SyntaxFactory.Token(SyntaxKind.OpenParenToken))
                                                    .WithCloseParenToken(
                                                        SyntaxFactory.Token(
                                                            SyntaxFactory.TriviaList(),
                                                            SyntaxKind.CloseParenToken,
                                                            SyntaxFactory.TriviaList(SyntaxFactory.Space))))
                                            .WithArrowToken(
                                                SyntaxFactory.Token(
                                                    SyntaxFactory.TriviaList(),
                                                    SyntaxKind.EqualsGreaterThanToken,
                                                    SyntaxFactory.TriviaList(SyntaxFactory.Space))))))
                                .WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken))
                                .WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken))))
                    .WithSemicolonToken(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(), SyntaxKind.SemicolonToken, SyntaxFactory.TriviaList()))
                        .WithTrailingTrivia(lastStatement.GetTrailingTrivia());

                editor.RemoveNode(node);
                editor.ReplaceNode(lastStatement, expression);
            }
        }

        private static MemberAccessExpressionSyntax GetAssertThrows(
            SyntaxTriviaList leadingTrivia, NameSyntax expectedExceptionIdentifier)
        {
            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(leadingTrivia, "Assert", SyntaxTriviaList.Empty)),
                SyntaxFactory.GenericName(SyntaxFactory.Identifier("Throws"))
                    .WithTypeArgumentList(
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(expectedExceptionIdentifier))
                            .WithLessThanToken(SyntaxFactory.Token(SyntaxKind.LessThanToken))
                            .WithGreaterThanToken(SyntaxFactory.Token(SyntaxKind.GreaterThanToken))));
        }
    }
}
