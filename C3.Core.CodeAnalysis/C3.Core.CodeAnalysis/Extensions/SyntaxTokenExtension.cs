using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace C3.Core.CodeAnalysis.Extensions
{
    public static class SyntaxTokenExtension
    {
        public static bool IsInterfaceModifier(this SyntaxToken syntaxToken)
        {
            return syntaxToken.IsKind(SyntaxKind.PartialKeyword) ||
                syntaxToken.IsKind(SyntaxKind.PublicKeyword) ||
                syntaxToken.IsKind(SyntaxKind.ProtectedKeyword) ||
                syntaxToken.IsKind(SyntaxKind.InternalKeyword) ||
                syntaxToken.IsKind(SyntaxKind.PrivateKeyword);
        }
        
        public static SyntaxTriviaList CreateCommentTrivia(string comments)
        {
            var commentPart1 = SyntaxFactory.Comment("/// <summary>");
            var commentPart2 = SyntaxFactory.Comment("\n");
            var commentPart3 = SyntaxFactory.Comment("/// " + comments);
            var commentPart4 = SyntaxFactory.Comment("\n");
            var commentPart5 = SyntaxFactory.Comment("/// </summary>");
            var commentPart6 = SyntaxFactory.Comment("\n");

            return SyntaxFactory.TriviaList(new[] { commentPart1, commentPart2, commentPart3, commentPart4, commentPart5, commentPart6 });
        }

    }
}
