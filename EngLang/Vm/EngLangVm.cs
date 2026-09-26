using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EngLang.Vm;

/// <summary>
/// Virtual machine which executes EngLang code.
/// </summary>
public class EngLangVm
{
    private ExecutionScope globalScope = new();
    private Dictionary<string, VmFunctionDeclaration> functions = new();

    public object GetVariableValue(string variableName)
    {
        return globalScope.GetVariableValue(variableName);
    }

    public VmVariableDeclaration? GetVariableDeclaration(string variableName)
    {
        return globalScope.GetVariableDeclaration(variableName);
    }

    public VmFunctionDeclaration? GetVmFunction(string functionName)
    {
        return functions.TryGetValue(functionName, out var func)
            ? func
            : null;
    }

    public void ExecuteCode(string sentence)
    {
        var parseResult = (ParagraphList)EngLangParser.Parse(sentence);
        foreach (var paragraph in parseResult.Paragraphs)
        {
            this.InterpretParagraph(paragraph);
        }
    }

    private void InterpretParagraph(Paragraph paragraph)
    {
        if (paragraph.Label is { } label)
        {
            var name = string.Join("", label.Markers);
            functions.Add(name + "@" + label.Parameters.Length,
                new VmFunctionDeclaration(name, label.Parameters.Length));
        }
        else
        {
            foreach (var statement in paragraph.Statements)
            {
                this.InterpretStatement(statement);
            }
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
                    if (this.globalScope.GetVariableDeclaration(declaration.Name) is { })
                    {
                        throw new EngLangRuntimeException($"Variable '{declaration.Name}' is already declared.");
                    }

                    var variableName = declaration.Name;
                    var variableType = new VmTypeIdentifierReference()
                    {
                        Type = declaration.TypeName.Name
                    };
                    globalScope.RegisterVariable(variableName, variableType);
                    globalScope.RegisterVariableValue(variableName,
                        declaration.Expression is { } expression ? EvaluateExpression(expression) : null);
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
                        if (globalScope.GetVariableDeclaration(variableName) is null)
                        {
                            globalScope.RegisterVariable(variableName, new() { Type = variableType });
                        }

                        var setter = globalScope.GetVariableSetter(assignmentExpression.Variable);
                        setter(EvaluateExpression(assignmentExpression.Expression));
                        return;
                    }
                    if (expressionStatement.Expression is InPlaceMathExpression inplaceMathExpression)
                    {
                        var variableName = inplaceMathExpression.TargetVariable.Name.Name;
                        var getter = globalScope.GetVariableGetter(inplaceMathExpression.TargetVariable);
                        var setter = globalScope.GetVariableSetter(inplaceMathExpression.TargetVariable);
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
                var conditionValue = EvaluateExpression(expressionStatement.Condition);
                if (AsBoolean(conditionValue))
                {
                    InterpretStatement(expressionStatement.Then);
                }

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
            case VariableExpression variableExpression:
                var getter = globalScope.GetVariableGetter(variableExpression.Identifier);
                return getter();
            case LogicalExpression logicalExpression:
                var first = (long)EvaluateExpression(logicalExpression.FirstOperand);
                switch (logicalExpression.Operator)
                {
                    case LogicalOperator.Equals:
                        {
                            var second = (long)EvaluateExpression(logicalExpression.SecondOperand);
                            return AsLogicalValue(first == second);
                        }
                    case LogicalOperator.NotEquals:
                        {
                            var second = (long)EvaluateExpression(logicalExpression.SecondOperand);
                            return AsLogicalValue(first != second);
                        }
                    case LogicalOperator.Less:
                        {
                            var second = (long)EvaluateExpression(logicalExpression.SecondOperand);
                            return AsLogicalValue(first < second);
                        }
                    case LogicalOperator.Greater:
                        {
                            var second = (long)EvaluateExpression(logicalExpression.SecondOperand);
                            return AsLogicalValue(first > second);
                        }
                    case LogicalOperator.LessOrEquals:
                        {
                            var second = (long)EvaluateExpression(logicalExpression.SecondOperand);
                            return AsLogicalValue(first <= second);
                        }
                    case LogicalOperator.GreaterOrEquals:
                        {
                            var second = (long)EvaluateExpression(logicalExpression.SecondOperand);
                            return AsLogicalValue(first >= second);
                        }
                    default:
                        throw new NotImplementedException($"Logical expression with operator {logicalExpression.Operator} is not supported.");
                }
        }
        throw new NotImplementedException($"Expression of type {expression.GetType().Name} is not supported.");
    }

    bool AsBoolean(object value)
    {
        return (long)value == 1;
    }
    long AsLogicalValue(bool value)
    {
        return value ? 1L : 0L;
    }
}
