using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EngLang.Vm;

/// <summary>
/// Virtual machine which executes EngLang code.
/// </summary>
public class EngLangVm
{
    private Dictionary<string, VmFunctionDeclaration> functions = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, VmTypeDeclaration> types = new(StringComparer.InvariantCultureIgnoreCase);

    internal ExecutionScope GlobalScope { get; } = new();

    public string[] GetVariableNames()
    {
        return GlobalScope.GetVariableNames();
    }

    public string[] GetFunctionNames()
    {
        return [.. functions.Keys];
    }

    public string[] GetTypeNames()
    {
        return [.. types.Keys];
    }

    public string[] GetConstNames()
    {
        return GlobalScope.GetConstNames();
    }

    public object GetVariableValue(string variableName)
    {
        return GlobalScope.GetVariableValue(variableName);
    }

    public VmVariableDeclaration? GetVariableDeclaration(string variableName)
    {
        return GlobalScope.GetVariableDeclaration(variableName);
    }

    public VmFunctionDeclaration? GetFunctionDeclaration(string functionName)
    {
        return functions.TryGetValue(functionName, out var func)
            ? func
            : null;
    }

    public VmTypeDeclaration? GetTypeDeclaration(string typeName)
    {
        return types.TryGetValue(typeName, out var type)
            ? type
            : null;
    }

    public VmConstDeclaration? GetConstDeclaration(string constName)
    {
        return GlobalScope.GetConstDeclaration(constName);
    }

    public void ExecuteCode(string sentence)
    {
        var parseResult = (ParagraphList)EngLangParser.Parse(sentence);
        foreach (var paragraph in parseResult.Paragraphs)
        {
            this.InterpretParagraph(GlobalScope, paragraph);
        }
    }

    private void InterpretParagraph(ExecutionScope scope, Paragraph paragraph)
    {
        if (paragraph.Label is { } label)
        {
            var name = string.Join("", label.Markers);
            VmFunctionDeclaration funcDeclaration = new(name, label.Parameters, [.. paragraph.Statements]);
            functions.Add(name.ToLowerInvariant() + "@" + label.Parameters.Length, funcDeclaration);
        }
        else
        {
            foreach (var statement in paragraph.Statements)
            {
                if (this.InterpretStatement(scope, statement))
                {
                    break;
                }
            }
        }
        
    }

    private bool InterpretStatement(ExecutionScope scope, Statement statement)
    {
        switch (statement)
        {
            case BlockStatement blockStatement:
                Debug.Assert(false, "Block statements are not yet supported.");
                break;
            case VariableDeclarationStatement variableDeclarationStatement:
                {
                    var declaration = variableDeclarationStatement.Declaration;
                    if (scope.GetVariableDeclaration(declaration.Name) is { })
                    {
                        throw new EngLangRuntimeException($"Variable '{declaration.Name}' is already declared.");
                    }

                    var variableName = declaration.Name;
                    var variableType = new VmTypeIdentifierReference()
                    {
                        Type = declaration.TypeName.Name
                    };
                    scope.RegisterVariable(variableName, variableType);
                    scope.RegisterVariableValue(variableName,
                        declaration.Expression is { } expression ? EvaluateExpression(scope, expression) : null);
                }
                break;
            case ShapeDeclarationStatement shapeDeclarationStatement:
                {
                    var declaration = shapeDeclarationStatement.Declaration;
                    var name = declaration.Name.Name;
                    Debug.Assert(!types.ContainsKey(name), $"Type {name} is already declared.");
                    Debug.Assert(declaration.BaseShapeName is null || declaration.BaseShapeName.Name == "thing", "Base shapes are not yet supported.");
                    Debug.Assert(declaration.BaseShapeName is null || !declaration.BaseShapeName.IsCollection, "Base shape collection are not yet supported.");
                    var typeDeclaration = new VmTypeDeclaration(name);
                    if (declaration.WellKnownSlots is { })
                    {
                        foreach (var slot in declaration.WellKnownSlots.Slots)
                        {
                            var slotName = slot.Name;
                            var slotType = slot.TypeName;
                            typeDeclaration.Properties.Add(slotName, new VmTypeDeclaration(slotType));
                        }
                    }

                    types.Add(name, typeDeclaration);
                }
                break;
            case ExpressionStatement expressionStatement:
                {
                    if (expressionStatement.Expression is AssignmentExpression assignmentExpression)
                    {
                        var variableName = assignmentExpression.Variable.Name.Name;
                        var variableType = assignmentExpression.Variable.Type.Name;
                        if (scope.GetVariableDeclaration(variableName) is null)
                        {
                            scope.RegisterVariable(variableName, new() { Type = variableType });
                        }

                        var setter = scope.GetVariableSetter(assignmentExpression.Variable);
                        setter(EvaluateExpression(scope, assignmentExpression.Expression));
                        break;
                    }
                    if (expressionStatement.Expression is InPlaceMathExpression inplaceMathExpression)
                    {
                        var variableName = inplaceMathExpression.TargetVariable.Name.Name;
                        var getter = scope.GetVariableGetter(inplaceMathExpression.TargetVariable);
                        var setter = scope.GetVariableSetter(inplaceMathExpression.TargetVariable);
                        var oldValue = (long)getter();
                        var addend = (long)EvaluateExpression(scope, inplaceMathExpression.ChangeValue);
                        var newValue = inplaceMathExpression.Operator switch
                        {
                            MathOperator.Plus => oldValue + addend,
                            MathOperator.Minus => oldValue - addend,
                            MathOperator.Multiply => oldValue * addend,
                            MathOperator.Divide => oldValue / addend,
                            _ => throw new InvalidOperationException($"Unsupported inplace math operation {inplaceMathExpression.Operator}"),
                        };
                        setter(newValue);
                        break;
                    }
                    Debug.Assert(false, $"Expression statements which are not assignment expressions are not yet supported. Expression type {expressionStatement.Expression.GetType().Name}");
                }
                break;
            case IfStatement expressionStatement:
                var conditionValue = EvaluateExpression(scope, expressionStatement.Condition);
                if (AsBoolean(conditionValue))
                {
                    if (InterpretStatement(scope, expressionStatement.Then))
                    {
                        return true;
                    }
                }

                break;
            case ResultStatement resultStatement:
                {
                    var fakeRange = new Yoakke.SynKit.Text.Range(new Yoakke.SynKit.Text.Position(0, 0), new Yoakke.SynKit.Text.Position(0, 0));
                    var resultSymbol = new SymbolName("result", fakeRange);
                    var resultTypeSymbol = new SymbolName("result", fakeRange);
                    var setter = scope.GetVariableSetter(new IdentifierReference(resultSymbol, resultTypeSymbol, null, fakeRange));
                    setter(EvaluateExpression(scope, resultStatement.Value));
                    return true;
                }
            case LabeledStatement labeledStatement:
                Debug.Assert(false, "Labeled statements are not yet supported.");
                break;
            case InvocationStatement invocationStatement:
                var marker = invocationStatement.Marker;
                const string IntoSuffix = " into";
                int parametersCount = invocationStatement.Parameters.Length;
                bool haveResult = false;
                if (marker.EndsWith(IntoSuffix))
                {
                    marker = marker.Substring(0, marker.Length - IntoSuffix.Length);
                    parametersCount--;
                    haveResult = true;
                }
                var func = GetFunctionDeclaration($"{marker}@{parametersCount}") ?? throw new InvalidOperationException($"Function `{marker}@{parametersCount}` not found");
                var functionScope = new ExecutionScope(scope);
                for (int i = 0; i < func.Parameters.Length; i++)
                {
                    IdentifierReference? p = func.Parameters[i];
                    var value = EvaluateExpression(scope, invocationStatement.Parameters[i]);
                    functionScope.RegisterVariable(p.Name.Name, new() { Type = p.Type.Name });
                    functionScope.RegisterVariableValue(p.Name.Name, value);
                }
                if (haveResult)
                {
                    functionScope.RegisterVariable("result", new() { Type = "result" });
                }
                foreach (var innerStatement in func.Statements)
                {
                    if (this.InterpretStatement(functionScope, innerStatement))
                    {
                        break;
                    }
                }

                if (haveResult)
                {
                    var resultValue = functionScope.GetVariableValue("result");
                    var resultVariable = (VariableExpression)invocationStatement.Parameters[^1];
                    var setter = scope.GetVariableSetter(resultVariable.Identifier);
                    setter(resultValue);
                }

                break;
            case PointerDeclarationStatement pointerDeclarationStatement:
                Debug.Assert(false, "Pointer declaration statements are not yet supported.");
                break;
            case ConstantDeclarationStatement constantDeclarationStatement:
                {
                    var name = constantDeclarationStatement.Identifier.Name.Name;
                    var value = EvaluateExpression(scope, constantDeclarationStatement.Value);
                    scope.RegisterConst(name, value);
                }
                break;
            case UnitAliasDeclarationStatement constantDeclarationStatement:
                Debug.Assert(false, "Unit alias declaration statements are not yet supported");
                break;
            case InvalidStatement invalidStatement:
                throw new InvalidOperationException($"Invalid statement `{invalidStatement}`");
            case Paragraph paragraph:
                this.InterpretParagraph(scope, paragraph);
                break;
            default:
                throw new NotImplementedException($"Statement of type {statement.GetType()} is not supported by converter");
        }

        return false;
    }

    private object? EvaluateExpression(ExecutionScope scope, Expression expression)
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
                var getter = scope.GetVariableGetter(variableExpression.Identifier);
                return getter();
            case LogicalExpression logicalExpression:
                var first = (long)EvaluateExpression(scope, logicalExpression.FirstOperand);
                switch (logicalExpression.Operator)
                {
                    case LogicalOperator.Equals:
                        {
                            var second = (long)EvaluateExpression(scope, logicalExpression.SecondOperand);
                            return AsLogicalValue(first == second);
                        }
                    case LogicalOperator.NotEquals:
                        {
                            var second = (long)EvaluateExpression(scope, logicalExpression.SecondOperand);
                            return AsLogicalValue(first != second);
                        }
                    case LogicalOperator.Less:
                        {
                            var second = (long)EvaluateExpression(scope, logicalExpression.SecondOperand);
                            return AsLogicalValue(first < second);
                        }
                    case LogicalOperator.Greater:
                        {
                            var second = (long)EvaluateExpression(scope, logicalExpression.SecondOperand);
                            return AsLogicalValue(first > second);
                        }
                    case LogicalOperator.LessOrEquals:
                        {
                            var second = (long)EvaluateExpression(scope, logicalExpression.SecondOperand);
                            return AsLogicalValue(first <= second);
                        }
                    case LogicalOperator.GreaterOrEquals:
                        {
                            var second = (long)EvaluateExpression(scope, logicalExpression.SecondOperand);
                            return AsLogicalValue(first >= second);
                        }
                    default:
                        throw new NotImplementedException($"Logical expression with operator {logicalExpression.Operator} is not supported.");
                }
            case MathExpression mathExpression:
                var firstOperand = (long)EvaluateExpression(scope, mathExpression.FirstOperand);
                var secondOperand = (long)EvaluateExpression(scope, mathExpression.SecondOperand);
                return mathExpression.Operator switch
                {
                    MathOperator.Plus => firstOperand + secondOperand,
                    MathOperator.Minus => firstOperand - secondOperand,
                    MathOperator.Multiply => firstOperand * secondOperand,
                    MathOperator.Divide => firstOperand / secondOperand,
                    _ => throw new NotImplementedException($"Math expression with operator {mathExpression.Operator} is not supported."),
                };
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
