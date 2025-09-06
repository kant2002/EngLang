using System;
using System.Diagnostics.Tracing;
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
            case ParagraphList paragraphList:
                ConvertParagraphList(builder, paragraphList);
                return builder.ToString();
            case Paragraph paragraph:
                ConvertParagraph(builder, paragraph);
                return builder.ToString();
            case Statement statement:
                ConvertStatement(builder, statement);
                return builder.ToString();
            case Expression expression:
                return ConvertExpression(expression);
            case VariableDeclaration variableDeclaration:
                return ConvertVariableDeclaration(variableDeclaration);
            case ShapeDeclaration shapeDeclaration:
                return ConvertShapeDeclaration(shapeDeclaration);
            case IdentifierReference identifierReference:
                return ConvertIdentifierReference(identifierReference);
            case TypeIdentifierReference typeIdentifierReference:
                return ConvertTypeIdentifierReference(typeIdentifierReference);
            default:
                throw new NotImplementedException($"Syntax node {node} is not supported");
        }
    }

    private string ConvertIdentifierReference(IdentifierReference identifierReference)
    {
        var identifier = ConvertToIdentifier(identifierReference.Name);
        if (identifierReference.Owner is not null)
        {
            return $"{ConvertIdentifierReference(identifierReference.Owner)}.{identifier}";
        }
        else
        {
            return identifier;
        }
    }

    private string ConvertTypeIdentifierReference(TypeIdentifierReference identifierReference)
    {
        var identifier = ConvertToIdentifier(identifierReference.Name);
        if (identifierReference.IsCollection)
        {
            return $"{identifier}[]";
        }
        else
        {
            return identifier;
        }
    }

    private string ConvertVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var result = new StringBuilder();
        result.Append(ConvertTypeIdentifierReference(variableDeclaration.TypeName));
        result.Append(' ');
        result.Append(ConvertToIdentifier(variableDeclaration.Name));
        if (variableDeclaration.Expression != null)
        {
            result.Append(" = ");
            result.Append(Convert(variableDeclaration.Expression));
        }

        return result.ToString();
    }

    private string ConvertShapeDeclaration(ShapeDeclaration variableDeclaration)
    {
        var result = new StringBuilder();
        result.Append("public class ");
        result.Append(ConvertToIdentifier(variableDeclaration.Name));
        if (variableDeclaration.BaseShapeName is { } baseShapeName)
        {
            result.Append(" : ");
            result.Append(ConvertToIdentifier(baseShapeName.Name));
        }

        result.AppendLine();
        result.AppendLine("{");
        if (variableDeclaration.WellKnownSlots != null)
        {
            foreach (var slot in variableDeclaration.WellKnownSlots.Slots)
            {
                result.AppendLine($"    public object {ConvertToIdentifier(slot.Name)};");
            }
        }

        result.Append("}");

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
            case InchLiteralExpression inchLiteralExpression:
                return $"{inchLiteralExpression.Value}";
            case RatioLiteralExpression ratioLiteralExpression:
                return $"{ratioLiteralExpression.Numerator} / {ratioLiteralExpression.Denominator}";
            case NullLiteralExpression nullLiteralExpression:
                return $"null";
            case ByteArrayLiteralExpression byteArrayLiteralExpression:
                return $"new byte[] {{ {string.Join(", ", byteArrayLiteralExpression.Value)} }}";
            case VariableExpression variableExpression:
                return $"{ConvertIdentifierReference(variableExpression.Identifier)}";
            case InPlaceAdditionExpression additionExpression:
                return $"{ConvertIdentifierReference(additionExpression.TargetVariable)} += {ConvertExpression(additionExpression.Addend)}";
            case InPlaceSubtractExpression substractExpression:
                return $"{ConvertIdentifierReference(substractExpression.TargetVariable)} -= {ConvertExpression(substractExpression.Subtrahend)}";
            case InPlaceMultiplyExpression multiplyExpression:
                return $"{ConvertIdentifierReference(multiplyExpression.TargetVariable)} *= {ConvertExpression(multiplyExpression.Factor)}";
            case InPlaceDivisionExpression divisionExpression:
                return $"{ConvertIdentifierReference(divisionExpression.TargetVariable)} /= {ConvertExpression(divisionExpression.Denominator)}";
            case AssignmentExpression assignmentExpression:
                return $"{ConvertIdentifierReference(assignmentExpression.Variable)} = {ConvertExpression(assignmentExpression.Expression)}";
            case InvalidExpression invalidExpression:
                return $"/* {invalidExpression.Code} */";
            case LogicalExpression equalityExpression:
                return $"{ConvertExpression(equalityExpression.FirstOperand)} {Convert(equalityExpression.Operator)} {ConvertExpression(equalityExpression.SecondOperand)}";
            case MathExpression mathExpression:
                return $"{ConvertExpression(mathExpression.FirstOperand)} {Convert(mathExpression.Operator)} {ConvertExpression(mathExpression.SecondOperand)}";
            case PosessiveExpression posessiveExpression:
                return $"{ConvertExpression(posessiveExpression.Owner)}.{ConvertIdentifierReference(posessiveExpression.Identifier)}";
            case InvocationExpression invocationExpression:
                return $"{invocationExpression.Marker}({string.Join(", ", invocationExpression.Parameters.Select(_ => ConvertExpression(_)))})";
            default:
                throw new NotImplementedException($"Expression of type {expression.GetType()} is not supported");
        }
    }

    private string GetExpressionType(Expression expression)
    {
        switch (expression)
        {
            case IntLiteralExpression intLiteralExpression:
                return "long";
            case StringLiteralExpression stringLiteralExpression:
                return "string";
            case InchLiteralExpression inchLiteralExpression:
                return "long";
            case RatioLiteralExpression ratioLiteralExpression:
                return "double";
            case NullLiteralExpression nullLiteralExpression:
                return $"object";
            case ByteArrayLiteralExpression byteArrayLiteralExpression:
                return $"byte[]";
            default:
                throw new NotImplementedException($"Expression of type {expression.GetType()} is not supported");
        }
    }

    private string Convert(LogicalOperator @operator)
    {
        return @operator switch
        {
            LogicalOperator.Equals => "==",
            LogicalOperator.NotEquals => "!=",
            LogicalOperator.Less => "<",
            LogicalOperator.LessOrEquals => "<=",
            LogicalOperator.Greater => ">",
            LogicalOperator.GreaterOrEquals => ">=",
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
            MathOperator.Concat => "+",
            _ => throw new NotImplementedException($"Operator {@operator} does not supported"),
        };
    }

    private void ConvertParagraph(IndentedStringBuilder builder, Paragraph paragraph)
    {
        var label = paragraph.Label;
        if (label is not null)
        {
            var primaryMarker = label.Markers.First();
            var parameterString = string.Join(", ", label.Parameters.Select(_ => _.Name.Name.Replace(" ", "_")));
            builder.AppendLine($"void {ConvertToIdentifier(primaryMarker)}({parameterString})");
            builder.OpenBraces();
        }

        foreach (var statement in paragraph.Statements)
        {
            ConvertStatement(builder, statement);
        }

        if (label is not null)
        {
            builder.CloseBraces();
        }
    }

    private void ConvertParagraphList(IndentedStringBuilder builder, ParagraphList paragraphList)
    {
        foreach (var paragraph in paragraphList.Paragraphs)
        {
            ConvertParagraph(builder, paragraph);
        }
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
                var variableDeclaration = variableDeclarationStatement.Declaration;
                builder.AppendLine($"{Convert(variableDeclaration)};");
                break;
            case ShapeDeclarationStatement shapeDeclarationStatement:
                var shapeDeclaration = shapeDeclarationStatement.Declaration;
                builder.AppendLine($"{Convert(shapeDeclaration)}");
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
                var parameterString = string.Join(", ", labeledStatement.Parameters.Select(_ => ConvertToIdentifier(_.Name)));
                var primaryMarker = labeledStatement.Marker.Markers.First();
                builder.AppendLine($"void {ConvertToIdentifier(primaryMarker)}({parameterString})");
                builder.OpenBraces();
                ConvertStatement(builder, labeledStatement.Statement);
                builder.CloseBraces();
                break;
            case InvocationStatement invocationStatement:
                var invocationParameterString = string.Join(", ", invocationStatement.Parameters.Select(Convert));
                var invocationStatementText = $"{ConvertToIdentifier(invocationStatement.Marker)}({invocationParameterString});";
                if (invocationStatement.ResultIdentifier != null)
                {
                    builder.AppendLine($"{Convert(invocationStatement.ResultIdentifier)} = {invocationStatementText}");
                    break;
                }

                builder.AppendLine(invocationStatementText);
                break;
            case PointerDeclarationStatement pointerDeclarationStatement:
                builder.AppendLine("public class " + ConvertIdentifierReference(pointerDeclarationStatement.PointerType)
                    + "(" + Convert(pointerDeclarationStatement.BaseType) + " " + ConvertToIdentifier(pointerDeclarationStatement.BaseType.Name) + "); // pointer");
                break;
            case ConstantDeclarationStatement constantDeclarationStatement:
                builder.AppendLine("public partial class constants");
                builder.OpenBraces();
                builder.AppendLine("public const " + GetExpressionType(constantDeclarationStatement.Value) + " " + ConvertIdentifierReference(constantDeclarationStatement.Identifier) + " = " + ConvertExpression(constantDeclarationStatement.Value) + ";");
                builder.CloseBraces();
                break;
            case UnitAliasDeclarationStatement constantDeclarationStatement:
                builder.AppendLine("public partial class constants");
                builder.OpenBraces();
                builder.AppendLine("public const " + GetExpressionType(constantDeclarationStatement.Value) + " " + ConvertIdentifierReference(constantDeclarationStatement.Identifier) + " = " + ConvertExpression(constantDeclarationStatement.Value) + " * " + ConvertIdentifierReference(constantDeclarationStatement.BaseUnit) + ";");
                builder.CloseBraces();
                break;
            case InvalidStatement invalidStatement:
                var invalidStatementString = string.Join(" ", invalidStatement.Tokens.Select(_ => _.Text));
                builder.AppendLine("#error " + invalidStatementString);
                break;
            case Paragraph paragraph:
                ConvertParagraph(builder, paragraph);
                break;
            default:
                throw new NotImplementedException($"Statement of type {statement.GetType()} is not supported by converter");
        }
    }

    private static string ConvertToIdentifier(SymbolName name) => ConvertToIdentifier(name.Name);

    private static string ConvertToIdentifier(string name)
    {
        string[] reservedWords = ["byte", "string"];
        if (reservedWords.Contains(name))
        {
            return "@" + name;
        }

        return name.Replace(' ', '_').Replace('-', '_').Replace('(', '_').Replace(')', '_').Replace('\'', '_');
    }
}
