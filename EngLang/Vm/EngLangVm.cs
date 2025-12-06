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
                var declaration = variableDeclarationStatement.Declaration;
                if (this.variables.ContainsKey(declaration.Name))
                {
                    throw new EngLangRuntimeException($"Variable '{declaration.Name}' is already declared.");
                }

                var variableType = new VmTypeIdentifierReference()
                {
                    Type = declaration.TypeName.Name
                };
                this.variables.Add(declaration.Name, new VmVariableDeclaration(declaration.Name, variableType));
                if (declaration.Expression is { } expression)
                {
                    this.variableValues.Add(declaration.Name, EvaluateExpression(expression));
                }
                else
                {
                    this.variableValues.Add(declaration.Name, null);
                }
                break;
            case ShapeDeclarationStatement shapeDeclarationStatement:
                Debug.Assert(false, "Shape declaration statements are not yet supported.");
                break;
            case ExpressionStatement expressionStatement:
                Debug.Assert(false, "Expression statements are not yet supported.");
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
