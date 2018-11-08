using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace C3.Core.CodeAnalysis.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeCodeFixProvider)), Shared]
    public class DateTimeCodeFixProvider : CodeFixProvider
    {
        private const string title = "Convert to DateTimeOffset";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CodeAnalysisConstants.DiagnosticIdDateTimeOffset); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (context.Diagnostics != null && context.Diagnostics.Count() > 0)
            {
                var diagnostic = context.Diagnostics.First();

                if (diagnostic != null && diagnostic.Location != null)
                {
                    var diagnosticSpan = diagnostic.Location.SourceSpan;
                    SemanticModel semanticModel = await context.Document.GetSemanticModelAsync().ConfigureAwait(false);

                    // Find the type declaration identified by the diagnostic.
                    var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();

                    if (declaration != null)
                    {
                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: title,
                                createChangedDocument: c => ConvertToDateTimeOffsetTypeAsync(context.Document, declaration, c),
                                equivalenceKey: title),
                            diagnostic);
                    }
                }
            }
        }

        private async Task<Document> ConvertToDateTimeOffsetTypeAsync(Document document, LocalDeclarationStatementSyntax localDeclaration, CancellationToken cancellationToken)
        {
            // #1 Replace DateTime x = new DateTime(); 
            // #2 Replace DateTime x = DateTime.Now; 
            // #3 var x = DateTime.Now; 
            // #4 var x = new DateTime(); 
            // #5 var x = new DateTime(); 
            // #6 var x = new DateTime(); 

            var firstToken = localDeclaration.GetFirstToken();
            var leadingTrivia = firstToken.LeadingTrivia;
            var trimmedLocal = localDeclaration.ReplaceToken(
                firstToken, firstToken.WithLeadingTrivia(SyntaxTriviaList.Empty));
      

            SyntaxNode finalResultSyntaxNode = DateTimeOffSetHelper.FixCode(localDeclaration);

            var newLocalDecl = finalResultSyntaxNode.WithAdditionalAnnotations(Formatter.Annotation);
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(localDeclaration, newLocalDecl);

            return document.WithSyntaxRoot(newRoot);
        }

        private SyntaxNode ReplaceTokenContext(SyntaxNode sourceToken, SyntaxToken oldToken, SyntaxToken newToken)
        {
            return sourceToken.ReplaceToken(oldToken, newToken);
        }
    }
}


