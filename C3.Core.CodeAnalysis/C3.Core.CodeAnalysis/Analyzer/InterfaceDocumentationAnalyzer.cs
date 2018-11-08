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
    public class InterfaceDocumentationAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString Title = "Please consider providing documentation/info for interface declaration";
        private static readonly LocalizableString MessageFormat = "Please consider providing documentation/info for interface declaration";
        private static readonly LocalizableString Description = "Please consider providing documentation/info for interface declaration";
        private const string Category = "Style";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(CodeAnalysisConstants.DiagnosticIdInterfaceDoc, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            
            var sym = context.SemanticModel.GetDeclaredSymbol(context.Node);
            var signsOfDocumentation = false; 

            if (sym != null && sym.DeclaringSyntaxReferences.Length > 0)
            {
                var anyLeadingDocs = sym.DeclaringSyntaxReferences.Any(x => x.GetSyntax().HasLeadingTrivia == true);

                foreach (var item in sym.DeclaringSyntaxReferences)
                {
                    var leadingTrivia = item.GetSyntax().GetLeadingTrivia();

                    signsOfDocumentation = leadingTrivia.AnyDocumentationTrivia();
                    if (signsOfDocumentation)
                    {
                        signsOfDocumentation = true;
                        return;
                    }
                }

                if (!signsOfDocumentation)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                }
            }
                        
            //var interfaceDeclaration = (InterfaceDeclarationSyntax) context.Node;
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


        //private void HandleInterfaceDocumentationTrace(SyntaxTriviaList syntaxTriviaList, SyntaxNodeAnalysisContext context)
        //{
        //    var docAvailableForInterfaceKeyword = syntaxTriviaList.AnyDocumentationTrivia();

        //    if (!docAvailableForInterfaceKeyword)
        //    {
        //        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        //    }
        //}
    }
}
