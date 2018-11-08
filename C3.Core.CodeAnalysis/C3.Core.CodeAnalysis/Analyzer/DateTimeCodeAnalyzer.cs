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
    public class DateTimeCodeAnalyzer : DiagnosticAnalyzer
    {        
        private const string DateTimeOffsetText = "DateTimeOffset";
        private static string Title = $"Usage warning : {DateTimeOffsetText} instead.";
        private static string MessageFormat = $"Please consider using {DateTimeOffsetText} which provide better support for date/time ";
        private static string Description = $"Consider changing current implementation to ${DateTimeOffsetText}";
        private const string Category = "Usage";

        private static DiagnosticDescriptor Rule = new
            DiagnosticDescriptor(CodeAnalysisConstants.DiagnosticIdDateTimeOffset, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalNodeDefinition, SyntaxKind.LocalDeclarationStatement);

            context.RegisterSyntaxNodeAction(AnalyzeMethodNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodNode(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration != null && methodDeclaration.ParameterList is BaseParameterListSyntax methodParameters)
            {
                foreach (var parameter in methodParameters.Parameters)
                {
                    var location = context.Node.GetLocation();
                    var message = $"Please convert {parameter.Identifier.Text}'s date type to DateTimeOffset";

                    switch (parameter?.Type)
                    {
                        case IdentifierNameSyntax parameterType:
                            var parameterTypings = parameterType.Identifier.ValueText;
                            FlagDateTimeUsageConcerns(context, parameterTypings, location, message);
                            break;
                        case NullableTypeSyntax nullableDateType:
                            HandleNullableDateTimeType(context, location, message, nullableDateType.ElementType);
                            break;
                        case ArrayTypeSyntax arrayDateTimeType:
                            HandleNullableDateTimeType(context, location, message, arrayDateTimeType.ElementType);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void HandleNullableDateTimeType(SyntaxNodeAnalysisContext context, Location location, string message, TypeSyntax typeSyntax)
        {
            if (typeSyntax is IdentifierNameSyntax nullableDateTimeIdentifier)
            {
                var paramType = nullableDateTimeIdentifier.Identifier.ValueText;
                FlagDateTimeUsageConcerns(context, paramType, location, message);
            }
        }

        private void AnalyzeLocalNodeDefinition(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax) context.Node;
            // #1. Type declaration scenario : DateTime d = new DateTime()
            var declaredType = localDeclaration?.GetVariableType();
            var location = context.Node.GetLocation();
            var declaredVariableName = localDeclaration?.GetVariableName();
            var nodeText = $"Please convert {declaredVariableName}'s date type to DateTimeOffset";

            if (!FlagDateTimeUsageConcerns(context, declaredType, location, nodeText))
            {
                // #2. var date = new DateTime() // 
                var assignmentType = localDeclaration.GetRightAssignmentDataType();
                FlagDateTimeUsageConcerns(context, assignmentType, location, nodeText);
            }
        }

        private bool FlagDateTimeUsageConcerns(SyntaxNodeAnalysisContext context, string declaredType, Location location, string diagnosticMessage)
        {
            if (!string.IsNullOrWhiteSpace(declaredType) && declaredType.ToLower().Equals(CodeAnalysisConstants.DateTimeDataTypeString))
            {
                NotifyAs(context, location, diagnosticMessage);
                return true;
            }
            return false;
        }

        private void NotifyAs(SyntaxNodeAnalysisContext context, Location location, string declaredVariableName)
        {
            var diagnostic = Diagnostic.Create(Rule, location, declaredVariableName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
