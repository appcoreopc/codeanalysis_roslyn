using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace C3.Core.CodeAnalysis.Extensions
{
    public static class VariableDeclarationSyntaxExtension
    {
        /// <summary>
        /// Get variable type from 'VariableDeclarationSyntax'
        /// </summary>
        /// <param name="variableDeclaration"></param>
        public static string GetVariableType(this VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration == null)
                throw new ArgumentNullException($"{nameof(variableDeclaration)} cannot be null.");

            switch (variableDeclaration?.Type)
            {
                case IdentifierNameSyntax declaredType:
                    return declaredType.Identifier.ValueText;
                case PredefinedTypeSyntax predefinedType:
                    return predefinedType?.Keyword.ValueText;
            }

            return null;
        }

        public static string GetVariableName(this VariableDeclarationSyntax localNodeDeclaration)
        {
            if (localNodeDeclaration == null)
                throw new ArgumentNullException($"{nameof(localNodeDeclaration)} cannot be null.");

            var identifierDeclaration = localNodeDeclaration?.Variables.FirstOrDefault();
            if (identifierDeclaration.Identifier != null)
            {
                return identifierDeclaration.Identifier.ValueText;
            }

            return null;
        }
    }
}
