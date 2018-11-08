using C3.Core.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace C3.Core.CodeAnalysis.Extensions
{
    public static class LocalDeclarationStatementSyntaxExtension
    {
        /// <summary>
        /// Get variable type from 'LocalDeclarationStatementSyntax'
        /// </summary>
        /// <param name="localNodeDeclaration"></param>
        public static string GetVariableType(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localNodeDeclaration)
        {
            if (localNodeDeclaration == null)
                return null;

            var identifierDeclaration = localNodeDeclaration?.Declaration;

            switch (localNodeDeclaration?.Declaration?.Type)
            {
                case IdentifierNameSyntax declaredType:
                    return declaredType.Identifier.ValueText;
                case PredefinedTypeSyntax predefinedType:
                    return predefinedType?.Keyword.ValueText;
            }
            return null;
        }

        public static string GetVariableName(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localNodeDeclaration)
        {
            if (localNodeDeclaration == null)
                return null;

            var identifierDeclaration = localNodeDeclaration?.Declaration?.Variables.FirstOrDefault();
            if (identifierDeclaration.Identifier != null)
            {
                return identifierDeclaration.Identifier.ValueText;
            }
            return null;
        }

        public static string GetRightAssignmentDataType(this Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax localNodeDeclaration)
        {
            if (localNodeDeclaration == null)
                return null;

            var identifierDeclaration = localNodeDeclaration?.Declaration?.Variables.FirstOrDefault() as VariableDeclaratorSyntax;

            switch (identifierDeclaration?.Initializer)
            {
                case EqualsValueClauseSyntax equalValueClause:
                    return TryGetEqualsValueClause(equalValueClause);
                default:
                    return null;
            }
        }

        private static string TryGetEqualsValueClause(EqualsValueClauseSyntax equalValueClause)
        {         
            return equalValueClause.GetAssignmentValueDataType();
        }
    }
}
