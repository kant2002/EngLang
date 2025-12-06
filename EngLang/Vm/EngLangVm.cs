using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EngLang.Vm;

/// <summary>
/// Virtual machine which executes EngLang code.
/// </summary>
public class EngLangVm
{
    private Dictionary<string, VmVariableDeclaration> variables = new();
    private Dictionary<string, object?> variableValues = new();

    public void ExecuteCode(string sentence)
    {
        var parseResult = (ParagraphList)EngLangParser.Parse(sentence);
        foreach (var paragraph in parseResult.Paragraphs)
        {
            this.InterpretParagraph(paragraph);
        }
    }

    public VmVariableDeclaration? GetVariableDeclaration(string variableName)
    {
        return this.variables.TryGetValue(variableName, out var declaration) ? declaration : null;
    }

    public object GetVariableValue(string variableName)
    {
        return variableValues.TryGetValue(variableName, out var value)
            ? value
            : throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
    }

    private void InterpretParagraph(Paragraph paragraph)
    {
        Debug.Assert(paragraph.Label is null, "Paragraph labels are not yet supported.");
        foreach (var statement in paragraph.Statements)
        {
            this.InterpretStatement(statement);
        }
    }

    private void InterpretStatement(Statement statement)
    {
        switch (statement)
        {
            case BlockStatement blockStatement:
                Debug.Assert(false, "Block statements are not yet supported.");
                break;
            case VariableDeclarationStatement variableDeclarationStatement:
                {
                    var declaration = variableDeclarationStatement.Declaration;
                    if (this.variables.ContainsKey(declaration.Name))
                    {
                        throw new EngLangRuntimeException($"Variable '{declaration.Name}' is already declared.");
                    }

                    var variableName = declaration.Name;
                    var variableType = new VmTypeIdentifierReference()
                    {
                        Type = declaration.TypeName.Name
                    };
                    RegisterVariable(variableName, variableType);
                    if (declaration.Expression is { } expression)
                    {
                        this.variableValues.Add(variableName, EvaluateExpression(expression));
                    }
                    else
                    {
                        this.variableValues.Add(variableName, null);
                    }
                }
                break;
            case ShapeDeclarationStatement shapeDeclarationStatement:
                Debug.Assert(false, "Shape declaration statements are not yet supported.");
                break;
            case ExpressionStatement expressionStatement:
                {
                    if (expressionStatement.Expression is AssignmentExpression assignmentExpression)
                    {
                        var variableName = assignmentExpression.Variable.Name.Name;
                        var variableType = assignmentExpression.Variable.Type.Name;
                        if (GetVariableDeclaration(variableName) is null)
                        {
                            RegisterVariable(variableName, new() { Type = variableType });
                        }

                        var setter = GetVariableSetter(assignmentExpression.Variable);
                        setter(EvaluateExpression(assignmentExpression.Expression));
                        return;
                    }
                    if (expressionStatement.Expression is InPlaceMathExpression inplaceMathExpression)
                    {
                        var variableName = inplaceMathExpression.TargetVariable.Name.Name;
                        var getter = GetVariableGetter(inplaceMathExpression.TargetVariable);
                        var setter = GetVariableSetter(inplaceMathExpression.TargetVariable);
                        var oldValue = (long)getter();
                        var addend = (long)EvaluateExpression(inplaceMathExpression.ChangeValue);
                        var newValue = inplaceMathExpression.Operator switch
                        {
                            MathOperator.Plus => oldValue + addend,
                            MathOperator.Minus => oldValue - addend,
                            MathOperator.Multiply => oldValue * addend,
                            MathOperator.Divide => oldValue / addend,
                            _ => throw new InvalidOperationException($"Unsupported inplace math operation {inplaceMathExpression.Operator}"),
                        };
                        setter(newValue);
                        return;
                    }
                    Debug.Assert(false, $"Expression statements which are not assignment expressions are not yet supported. Expression type {expressionStatement.Expression.GetType().Name}");
                }
                break;
            case IfStatement expressionStatement:
                Debug.Assert(false, "If statements are not yet supported.");
                break;
            case ResultStatement resultStatement:
                Debug.Assert(false, "Result statements are not yet supported.");
                break;
            case LabeledStatement labeledStatement:
                Debug.Assert(false, "Labeled statements are not yet supported.");
                break;
            case InvocationStatement invocationStatement:
                Debug.Assert(false, "Invocation statements are not yet supported.");
                break;
            case PointerDeclarationStatement pointerDeclarationStatement:
                Debug.Assert(false, "Pointer declaration statements are not yet supported.");
                break;
            case ConstantDeclarationStatement constantDeclarationStatement:
                Debug.Assert(false, "Constant declaration statements are not yet supported");
                break;
            case UnitAliasDeclarationStatement constantDeclarationStatement:
                Debug.Assert(false, "Unit alias declaration statements are not yet supported");
                break;
            case InvalidStatement invalidStatement:
                throw new InvalidOperationException($"Invalid statement `{invalidStatement}`");
            case Paragraph paragraph:
                this.InterpretParagraph(paragraph);
                break;
            default:
                throw new NotImplementedException($"Statement of type {statement.GetType()} is not supported by converter");
        }
    }

    private void RegisterVariable(string variableName, VmTypeIdentifierReference variableType)
    {
        this.variables.Add(variableName, new VmVariableDeclaration(variableName, variableType));
    }

    private Action<object?> GetVariableSetter(IdentifierReference variable)
    {
        Debug.Assert(variable.Owner is null, "Variable owners are not yet supported.");
        Debug.Assert(variable.Type is null || variable.Type.Name == variable.Name.Name, "Variable types is not supported");
        var variableName = variable.Name.Name;
        return (Action<object?>)(value =>
        {
            if (!this.variables.ContainsKey(variableName))
            {
                throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
            }
            this.variableValues[variableName] = value;
        });
    }

    private Func<object?> GetVariableGetter(IdentifierReference variable)
    {
        Debug.Assert(variable.Owner is null, "Variable owners are not yet supported.");
        Debug.Assert(variable.Type is null || variable.Type.Name == variable.Name.Name, "Variable types is not supported");
        var variableName = variable.Name.Name;
        return (Func<object?>)(() =>
        {
            if (!this.variables.ContainsKey(variableName))
            {
                throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
            }
            return this.variableValues.TryGetValue(variableName, out var value) ? value : null;
        });
    }

    private object? EvaluateExpression(Expression expression)
    {
        switch (expression)
        {
            case NullLiteralExpression nullLiteralExpression:
                return null;
            case IntLiteralExpression intLiteralExpression:
                return intLiteralExpression.Value;
            case StringLiteralExpression stringLiteralExpression:
                return stringLiteralExpression.Value;
        }
        throw new NotImplementedException($"Expression of type {expression.GetType().Name} is not supported.");
    }
}
