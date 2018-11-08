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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodInterfaceDocCodeFixProvider)), Shared]
    public class MethodInterfaceDocCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add some docs to this method";
        private const string interfaceDefinitionDocMessage = "Please provide documentation for inteface's method definition";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CodeAnalysisConstants.DiagnosticIdMethodInterfaceDoc); }
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
                    
                    var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();

                    if (declaration != null)
                    {
                        
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
            
            MemberDeclarationSyntax newMemberNode = null;
            MemberDeclarationSyntax currentMemberNode = null;

            if (interfaceDefinition != null && interfaceDefinition.Members != null)
            {
                foreach (var methodDefinition in interfaceDefinition.Members)
                {
                    if (!methodDefinition.GetLeadingTrivia().AnyDocumentationTrivia())
                    {
                        currentMemberNode = methodDefinition;
                        newMemberNode = methodDefinition.WithLeadingTrivia(SyntaxTokenExtension.CreateCommentTrivia(interfaceDefinitionDocMessage));
                        break;
                    }
                }
            }
            
            var formattedLocal = newMemberNode.WithAdditionalAnnotations(Formatter.Annotation);
            
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(currentMemberNode, formattedLocal);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}


