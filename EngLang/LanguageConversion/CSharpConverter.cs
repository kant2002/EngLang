using System;
using System.Linq;
using System.Text;

namespace EngLang.LanguageConversion;

public class CSharpConverter : ILanguageConverter
{
    public string Convert(SyntaxNode node)
    {
        IndentedStringBuilder builder = new(string.Empty);
        switch (node)
        {
            case Statement statement:
                ConvertStatement(builder, statement);
                return builder.ToString();
            case Expression expression:
                return ConvertExpression(expression);
            case VariableDeclaration variableDeclaration:
                return ConvertVariableDeclaration(variableDeclaration);
            case IdentifierReference identifierReference:
                return ConvertToIdentifier(identifierReference.Name);
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
            case VariableExpression variableExpression:
                return $"{ConvertToIdentifier(variableExpression.Identifier.Name)}";
            case InPlaceAdditionExpression additionExpression:
                return $"{ConvertToIdentifier(additionExpression.TargetVariable.Name)} += {ConvertExpression(additionExpression.Addend)}";
            case InPlaceSubtractExpression substractExpression:
                return $"{ConvertToIdentifier(substractExpression.TargetVariable.Name)} -= {ConvertExpression(substractExpression.Subtrahend)}";
            case InPlaceMultiplyExpression multiplyExpression:
                return $"{ConvertToIdentifier(multiplyExpression.TargetVariable.Name)} *= {ConvertExpression(multiplyExpression.Factor)}";
            case InPlaceDivisionExpression divisionExpression:
                return $"{ConvertToIdentifier(divisionExpression.TargetVariable.Name)} /= {ConvertExpression(divisionExpression.Denominator)}";
            case AssignmentExpression assignmentExpression:
                return $"{ConvertToIdentifier(assignmentExpression.Variable.Name)} = {ConvertExpression(assignmentExpression.Expression)}";
            case LogicalExpression equalityExpression:
                return $"{ConvertExpression(equalityExpression.FirstOperand)} {Convert(equalityExpression.Operator)} {ConvertExpression(equalityExpression.SecondOperand)}";
            case MathExpression mathExpression:
                return $"{ConvertExpression(mathExpression.FirstOperand)} {Convert(mathExpression.Operator)} {ConvertExpression(mathExpression.SecondOperand)}";
            default:
                throw new NotImplementedException($"Expression of type {expression.GetType()} is not supported");
        }
    }

    private string Convert(LogicalOperator @operator)
    {
        return @operator switch
        {
            LogicalOperator.Equals => "==",
            LogicalOperator.Less => "<",
            LogicalOperator.Greater => ">",
            _ => throw new NotImplementedException($"Operator {@operator} does not supported"),
        };
    }

    private string Convert(MathOperator @operator)
    {
        return @operator switch
        {
            MathOperator.Plus => "+",
            MathOperator.Minus => "-",
            MathOperator.Multiply => "*",
            MathOperator.Divide => "/",
            _ => throw new NotImplementedException($"Operator {@operator} does not supported"),
        };
    }

    private void ConvertStatement(IndentedStringBuilder builder, Statement statement)
    {
        switch (statement)
        {
            case BlockStatement blockStatement:
                foreach (var childStatement in blockStatement.Statements)
                {
                    ConvertStatement(builder, childStatement);
                }

                break;
            case VariableDeclarationStatement variableDeclarationStatement:
                var declaration = variableDeclarationStatement.Declaration;
                builder.AppendLine($"{Convert(declaration)};");
                break;
            case ExpressionStatement expressionStatement:
                var additionExpression = expressionStatement.Expression;
                builder.AppendLine($"{Convert(additionExpression)};");
                break;
            case IfStatement expressionStatement:
                var ifConditionExpression = expressionStatement.Condition;
                builder.AppendLine($"if ({Convert(ifConditionExpression)}) {{");
                builder.PushIndent();
                ConvertStatement(builder, expressionStatement.Then);
                builder.PopIndent();
                builder.AppendLine("}");
                break;
            case ResultStatement resultStatement:
                builder.AppendLine($"return {Convert(resultStatement.Value)};");
                break;
            case LabeledStatement labeledStatement:
                var parameterString = string.Join(", ", labeledStatement.Parameters.Select(_ => _.Name.Replace(" ", "_")));
                builder.AppendLine($"void {labeledStatement.Marker.Replace(" ", "_")}({parameterString})");
                builder.OpenBraces();
                ConvertStatement(builder, labeledStatement.Statement);
                builder.CloseBraces();
                break;
            case InvocationStatement invocationStatement:
                var invocationParameterString = string.Join(", ", invocationStatement.Parameters.Select(_ => _.Name.Replace(" ", "_")));
                var invocationStatementText = $"{invocationStatement.Marker.Replace(" ", "_")}({invocationParameterString});";
                if (invocationStatement.ResultIdentifier != null)
                {
                    builder.AppendLine($"{Convert(invocationStatement.ResultIdentifier)} = {invocationStatementText}");
                    break;
                }

                builder.AppendLine(invocationStatementText);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private static string ConvertToIdentifier(string name)
    {
        return name.Replace(' ', '_');
    }
}
