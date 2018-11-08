using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using C3.Core.CodeAnalysis.Extensions;
using C3.Core.CodeAnalysis;
using System.Linq;

namespace C3.CodeAnalysis.Net.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceMethodDocAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString Title = "Please provide documentation/info for interface's method";
        private static readonly LocalizableString MessageFormat = Title;
        private static readonly LocalizableString Description = Title;
        private const string Category = "Style";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(CodeAnalysisConstants.DiagnosticIdMethodInterfaceDoc, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax) context.Node;

            if (interfaceDeclaration != null)
            {
                foreach (var memberMethod in interfaceDeclaration.Members)
                {
                    var anyMethodDocs = memberMethod.GetLeadingTrivia().AnyDocumentationTrivia();
                    var methodLocation = memberMethod.GetLocation();

                    if (!anyMethodDocs)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, methodLocation));
                        return;
                    }
                }
            }

            //// Common cases for interface modifier //
            //// To make code analysis faster, we try to use first modifier 
            //if (interfaceDeclaration.Modifiers.Count > 0)
            //{
            //    var declaredInterface = interfaceDeclaration.Modifiers.FirstOrDefault(x => x.IsInterfaceModifier());
            //    if (declaredInterface != null)
            //        HandleInterfaceDocumentationTrace(declaredInterface.LeadingTrivia, context);
            //}
            //else
            //{
            //    // normal interface definition 
            //    if (interfaceDeclaration.Keyword.HasLeadingTrivia &&
            //    interfaceDeclaration.Keyword.LeadingTrivia.Count > 0)
            //    {
            //        HandleInterfaceDocumentationTrace(interfaceDeclaration.Keyword.LeadingTrivia, context);
            //    }
            //}
        }

        private void HandleInterfaceDocumentationTrace(SyntaxTriviaList syntaxTriviaList, SyntaxNodeAnalysisContext context)
        {
            var docAvailableForInterfaceKeyword = syntaxTriviaList.AnyDocumentationTrivia();

            if (!docAvailableForInterfaceKeyword)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}
