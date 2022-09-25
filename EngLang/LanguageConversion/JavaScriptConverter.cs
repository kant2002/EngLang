using System;
using System.Linq;
using System.Text;

namespace EngLang.LanguageConversion;

public class JavaScriptConverter : ILanguageConverter
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
        result.Append("let ");
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
                throw new NotImplementedException();
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
            case ExpressionStatement expressionStatement:
                var additionExpression = expressionStatement.Expression;
                return $"{Convert(additionExpression)};";
            case IfStatement expressionStatement:
                var ifConditionExpression = expressionStatement.Condition;
                return @$"if ({Convert(ifConditionExpression)}) {{" + Environment.NewLine
                    + "    " + Convert(expressionStatement.Then) + Environment.NewLine
                    + "}" + Environment.NewLine;
            case ResultStatement resultStatement:
                return $"return {Convert(resultStatement.Value)};";
            case LabeledStatement labeledStatement:
                var parameterString = string.Join(", ", labeledStatement.Parameters.Select(_ => _.Name.Replace(" ", "_")));
                return $"function {labeledStatement.Marker.Replace(" ", "_")}({parameterString}) {{" + Environment.NewLine
                    + "    " + Convert(labeledStatement.Statement)
                    + "}" + Environment.NewLine;
            default:
                throw new NotImplementedException();
        }
    }

    private string ConvertToIdentifier(string name)
    {
        return name.Replace(' ', '_');
    }
}
