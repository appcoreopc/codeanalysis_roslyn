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
using C3.Core.CodeAnalysis.Extensions;

namespace C3.Core.CodeAnalysis.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceDocCodeFixProvider)), Shared]
    public class InterfaceDocCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add documentation to interface";
        private const string interfaceDefinitionDocMessage = "Please provide documentation for this interface definition";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CodeAnalysisConstants.DiagnosticIdInterfaceDoc); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
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

                    // Find the type declaration identified by the diagnostic.
                    var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();

                    if (declaration != null)
                    {
                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: title,
                                createChangedDocument: c => ParseDocsForInterfaceDefinition(context.Document, declaration, c),
                                equivalenceKey: title),
                            diagnostic);
                    }
                }
            }
        }

        private async Task<Document> ParseDocsForInterfaceDefinition(Document document, InterfaceDeclarationSyntax interfaceDefinition, CancellationToken cancellationToken)
        {

            var firstToken = interfaceDefinition.GetFirstToken();
            var leadingTrivia = firstToken.LeadingTrivia;
            var trimmedLocal = interfaceDefinition.ReplaceToken(
                firstToken, firstToken.WithLeadingTrivia(SyntaxTriviaList.Empty));
            
            var newLocal = trimmedLocal.WithLeadingTrivia(SyntaxTokenExtension.CreateCommentTrivia(interfaceDefinitionDocMessage));
                        
            var formattedLocal = newLocal.WithAdditionalAnnotations(Formatter.Annotation);
                        
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(interfaceDefinition, formattedLocal);

            // Return document with transformed tree.
            return document.WithSyntaxRoot(newRoot);
        }
    }
}


