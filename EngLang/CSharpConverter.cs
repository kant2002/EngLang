using System;
using System.Text;

namespace EngLang;

public class CSharpConverter: ILanguageConverter
{
    public string Convert(SyntaxNode node)
    {
        switch (node)
        {
            case Statement statement:
                return ConvertStatement(statement);
            case Expression expression:
                return ConvertExpression(expression);
            case VariableDeclaration variableDeclaration:
                return ConvertVariableDeclaration(variableDeclaration);
            default:
                throw new NotImplementedException();
        }
    }

    private string ConvertVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var result = new StringBuilder();
        result.Append(ConvertToIdentifier(variableDeclaration.TypeName.Name));
        result.Append(' ');
        result.Append(ConvertToIdentifier(variableDeclaration.Name));
        if (variableDeclaration.Expression != null)
        {
            result.Append(" = ");
            result.Append(Convert(variableDeclaration.Expression));
        }

        return result.ToString();
    }

    private string ConvertExpression(Expression expression)
    {
        switch (expression)
        {
            case IntLiteralExpression intLiteralExpression:
                return intLiteralExpression.Value.ToString();
            case StringLiteralExpression stringLiteralExpression:
                return $"\"{stringLiteralExpression.Value}\"";
            case AdditionExpression additionExpression:
                return $"{ConvertToIdentifier(additionExpression.TargetVariable.Name)} += {ConvertExpression(additionExpression.Addend)}";
            case SubstractExpression substractExpression:
                return $"{ConvertToIdentifier(substractExpression.TargetVariable.Name)} -= {ConvertExpression(substractExpression.Subtrahend)}";
            case MultiplyExpression multiplyExpression:
                return $"{ConvertToIdentifier(multiplyExpression.TargetVariable.Name)} *= {ConvertExpression(multiplyExpression.Factor)}";
            case DivisionExpression divisionExpression:
                return $"{ConvertToIdentifier(divisionExpression.TargetVariable.Name)} /= {ConvertExpression(divisionExpression.Denominator)}";
            case AssignmentExpression assignmentExpression:
                return $"{ConvertToIdentifier(assignmentExpression.Variable.Name)} = {ConvertExpression(assignmentExpression.Expression)}";
            default:
                throw new NotImplementedException();
        }
    }

    private string ConvertStatement(Statement statement)
    {
        switch (statement)
        {
            case BlockStatement blockStatement:
                StringBuilder builder = new();
                foreach (var childStatement in blockStatement.Statements)
                {
                    builder.AppendLine(ConvertStatement(childStatement));
                }

                return builder.ToString();
            case VariableDeclarationStatement variableDeclarationStatement:
                var declaration = variableDeclarationStatement.Declaration;
                return $"{Convert(declaration)};";
            case AssignmentStatement assignmentStatement:
                var assignmentExpression = assignmentStatement.Expression;
                return $"{Convert(assignmentExpression)};";
            default:
                throw new NotImplementedException();
        }
    }

    private string ConvertToIdentifier(string name)
    {
        return name.Replace(' ', '_');
    }
}
