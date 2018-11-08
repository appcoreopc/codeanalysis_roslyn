using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace C3.Core.CodeAnalysis.Extensions
{
    public static class EqualsValueClauseSyntaxExtension
    {
        public static string GetAssignmentValueDataType(this EqualsValueClauseSyntax equalValueClause)
        {
            //return equalValueClause.GetMemberAccessExpressionType() ?? equalValueClause.GetNewObjectCreationDataType() ?? null;

            switch (equalValueClause.Value)
            {
                case InvocationExpressionSyntax invokeExpressionClause:
                    if (invokeExpressionClause.Expression is MemberAccessExpressionSyntax invocationMemberAccessClause)
                        //goto case MemberAccessExpressionSyntax;
                        if (invocationMemberAccessClause.Expression is IdentifierNameSyntax invokedIndentifierName)
                            return invokedIndentifierName.Identifier.ValueText;
                    break;
                case MemberAccessExpressionSyntax assignmentNode:
                    if (assignmentNode.Expression is IdentifierNameSyntax memberAccessClause)
                        return memberAccessClause.Identifier.ValueText;
                    break;
                case ObjectCreationExpressionSyntax creationObjectClause:
                    if (creationObjectClause.Type is IdentifierNameSyntax identifer)
                        return identifer.Identifier.ValueText;
                    break;
                default:
                    return null;
            }
            return null;
        }
        
        public static string GetMemberAccessExpressionType(this EqualsValueClauseSyntax equalValueClause)
        {
            if (equalValueClause.Value is MemberAccessExpressionSyntax assignmentNode && assignmentNode is MemberAccessExpressionSyntax)
            {
                if (assignmentNode.Expression is IdentifierNameSyntax memberAccessClause)
                {
                    return memberAccessClause.Identifier.ValueText;
                }
            }
            return null;
        }

        public static string GetNewObjectCreationDataType(this EqualsValueClauseSyntax equalsValueClause)
        {
            if (equalsValueClause.Value is ObjectCreationExpressionSyntax creationObjectClause)
            {
                if (creationObjectClause.Type is IdentifierNameSyntax identifer)
                {
                    return identifer.Identifier.ValueText;
                }
            }
            return null;
        }
    }
}