using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace C3.Core.CodeAnalysis.Extensions
{
    public static class SyntaxTriviaExtension
    {
        public static bool IsSingleLineComment(this SyntaxTrivia trivia)
        {
            return (int)(trivia.Kind()) == 8541;
        }

        public static bool IsDocumentationAvailable(this SyntaxTrivia trivia)
        {
            return trivia.IsSingleLineDocComment() || trivia.IsMultiLineDocumentationCommentTrivia();
        }

        public static bool IsSingleLineDocComment(this SyntaxTrivia trivia)
        {
            return (int)trivia.Kind() == 8544;
        }

        public static bool IsMultiLineDocumentationCommentTrivia(this SyntaxTrivia trivia)
        {
            return (int)trivia.Kind() == 8545;
        }
    }
}
