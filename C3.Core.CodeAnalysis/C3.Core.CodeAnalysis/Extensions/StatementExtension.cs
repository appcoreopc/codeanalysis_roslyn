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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Simplification;

namespace C3.Core.CodeAnalysis.Extensions
{
    class StatementExtension
    {

       public ExpressionStatementSyntax CreateStatement(string identiferName, string typeName)
        {
            var variableIdentifier = SyntaxFactory.IdentifierName(identiferName);
            var classIdentifier = SyntaxFactory.IdentifierName(typeName);
            var objectCreationExpression = SyntaxFactory.ObjectCreationExpression(classIdentifier, SyntaxFactory.ArgumentList(), null);
            var assignment = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, variableIdentifier, objectCreationExpression);
            return SyntaxFactory.ExpressionStatement(assignment).WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
