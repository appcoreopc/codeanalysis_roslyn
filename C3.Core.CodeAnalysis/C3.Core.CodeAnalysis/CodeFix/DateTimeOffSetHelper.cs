using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;
using C3.CodeAnalysis.Net.Analyzer;
using C3.Core.CodeAnalysis.Extensions;
using System.Collections.Generic;

namespace C3.Core.CodeAnalysis.CodeFix
{
    class DateTimeOffSetHelper
    {                
        public static SyntaxNode FixCode(SyntaxNode localDeclaration)
        {
            SyntaxNode finalResultSyntaxNode = SyntaxFactory.EmptyStatement();

            var targetNewToken = SyntaxFactory.Identifier(CodeAnalysisConstants.DateTimeOffsetConstant);

            var dateTimeTokenOccurance = localDeclaration.DescendantTokens().Where(x => x.ValueText.ToLower() == CodeAnalysisConstants.DateTimeDataTypeString);

            if (dateTimeTokenOccurance != null)
            {
                var count = dateTimeTokenOccurance.Count();

                if (count > 0)
                {
                    var tokenToReplace = localDeclaration.DescendantTokens().Where(x => x.ValueText.ToLower() == CodeAnalysisConstants.DateTimeDataTypeString).FirstOrDefault();

                    if (tokenToReplace != null)
                    {
                        var dateTimeDeclaration = ReplaceTokenContext(localDeclaration, tokenToReplace, targetNewToken);

                        tokenToReplace = dateTimeDeclaration.DescendantTokens().Where(x => x.ValueText.ToLower() == CodeAnalysisConstants.DateTimeDataTypeString).FirstOrDefault();

                        if (tokenToReplace != null && !tokenToReplace.IsKind(SyntaxKind.None))
                        {
                            var InitializerReplaceResult = ReplaceTokenContext(dateTimeDeclaration, tokenToReplace, targetNewToken);
                            finalResultSyntaxNode = InitializerReplaceResult;
                        }
                        else
                            finalResultSyntaxNode = dateTimeDeclaration;
                    }
                }

            }

            return finalResultSyntaxNode;
        }

        private static SyntaxNode ReplaceTokenContext(SyntaxNode sourceToken, SyntaxToken oldToken, SyntaxToken newToken)
        {
            return sourceToken.ReplaceToken(oldToken, newToken);
        }
    }
}
