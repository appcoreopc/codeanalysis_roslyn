using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using C3.Core.CodeAnalysis.Extensions;
using C3.Core.CodeAnalysis;

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
                    
                    if (!anyMethodDocs)
                    {
                        var methodLocation = memberMethod.GetLocation();
                        context.ReportDiagnostic(Diagnostic.Create(Rule, methodLocation));
                        return;
                    }
                }
            }
        }
    }
}
