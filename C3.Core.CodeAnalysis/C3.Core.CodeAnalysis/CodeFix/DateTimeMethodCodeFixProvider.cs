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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeCodeFixProvider)), Shared]
    public class DateTimeMethodCodeFixProvider : CodeFixProvider
    {
        private const string title = "Convert to DateTimeOffset";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CodeAnalysisConstants.DiagnosticIdDateTimeUseOnMethodOffset); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ConvertToDateTimeOffsetTypeAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ConvertToDateTimeOffsetTypeAsync(Document document, MethodDeclarationSyntax localDeclaration, CancellationToken cancellationToken)
        {

            SyntaxNode finalResultSyntaxNode = DateTimeOffSetHelper.FixCode(localDeclaration);

            var newLocalDecl = finalResultSyntaxNode.WithAdditionalAnnotations(Formatter.Annotation);
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(localDeclaration, newLocalDecl);

            return document.WithSyntaxRoot(newRoot);
          
        }
    }
}
