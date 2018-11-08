using Microsoft.CodeAnalysis;
using System.Linq;

namespace C3.Core.CodeAnalysis.Extensions
{
    public static class SyntaxTriviaListExtension
    {
        public static bool AnyDocumentationTrivia(this SyntaxTriviaList syntaxTriviaList)
        {
            return syntaxTriviaList.OfType<SyntaxTrivia>().Any(x => x.IsDocumentationAvailable());    
        }
    }
}
