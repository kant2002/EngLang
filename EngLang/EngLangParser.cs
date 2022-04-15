using System;
using System.Collections.Immutable;
using System.Linq;

namespace EngLang;

public class EngLangParser
{
    public static SyntaxNode Parse(string sourceCode)
    {
        if (sourceCode.Contains('.'))
        {
            var statmementTexts = sourceCode.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var statementsArray = statmementTexts
                .Select(ParseNode)
                .Select(ConvertToStatement)
                .ToArray();
            var statements = ImmutableList.Create<Statement>(statementsArray);
            return new BlockStatement(statements);
        }

        return ParseNode(sourceCode);
    }

    private static Statement ConvertToStatement(SyntaxNode node)
    {
        return node switch
        {
            VariableDeclaration variableDeclaration => new VariableDeclarationStatement(variableDeclaration),
            _ => throw new InvalidOperationException(),
        };
    }

    private static SyntaxNode ParseNode(string sourceCode)
    {
        var parts = sourceCode.Split(' ');
        switch (parts[0])
        {
            case "a":
            case "an":
                var variableReference = ParseIdentifierReference(string.Join(' ', parts.Skip(1)));
                return variableReference;
            case "the":
                var variableDeclaration = ParseVariableDeclaration(string.Join(' ', parts.Skip(1)));
                return variableDeclaration;
            case "add":
            case "substract":
            case "multiply":
            case "divide":
                return ParseExpression(sourceCode);
            default:
                throw new NotImplementedException();
        }
    }

    private static IdentifierReference ParseIdentifierReference(string content)
    {
        var variableName = content;
        var identifierReference = new IdentifierReference(variableName);
        return identifierReference;
    }

    private static VariableDeclaration ParseVariableDeclaration(string content)
    {
        var parts = content.Split(" is ");
        var variableName = parts[0];
        var variableSpecification = parts[1];
        var specificationParts = variableSpecification.Split(" equal to");
        var syntaxNode = Parse(specificationParts[0]);
        var identifierReference = syntaxNode as IdentifierReference;
        if (identifierReference == null)
        {
            throw new InvalidOperationException($"Identifier expected. {variableSpecification} given");
        }

        Expression? expression = null;
        if (specificationParts.Length > 1)
        {
            expression = ParseLiteralExpression(specificationParts[1].Trim());
        }

        var variableDeclaration = new VariableDeclaration(variableName, identifierReference, expression);
        return variableDeclaration;
    }

    private static Expression ParseExpression(string expressionString)
    {
        var parts = expressionString.Split(new[] { ' ' }, 2);
        switch (parts[0])
        {
            case "add":
                {
                    var subParts = parts[1].Split(" to");
                    var addend = ParseLiteralExpression(subParts[0]);
                    var target = (IdentifierReference)ParseNode(subParts[1].Trim());
                    return new AdditionExpression(addend, target);
                }
            case "substract":
                {
                    var subParts = parts[1].Split(" from");
                    var addend = ParseLiteralExpression(subParts[0]);
                    var target = (IdentifierReference)ParseNode(subParts[1].Trim());
                    return new SubstractExpression(addend, target);
                }
            case "multiply":
                {
                    var subParts = parts[1].Split(" by");
                    var target = (IdentifierReference)ParseNode(subParts[0].Trim());
                    var addend = ParseLiteralExpression(subParts[1].Trim());
                    return new MultiplyExpression(addend, target);
                }
            case "divide":
                {
                    var subParts = parts[1].Split(" by");
                    var target = (IdentifierReference)ParseNode(subParts[0].Trim());
                    var addend = ParseLiteralExpression(subParts[1].Trim());
                    return new DivisionExpression(addend, target);
                }
            default:
                throw new NotImplementedException();
        }
    }

    private static Expression ParseLiteralExpression(string expressionString)
    {
        if (expressionString.StartsWith('"'))
        {
            return new StringLiteralExpression(expressionString.Trim('"'));
        }

        if (char.IsNumber(expressionString[0]))
        {
            return new IntLiteralExpression(int.Parse(expressionString));
        }

        throw new InvalidOperationException($"Cannot parse expression '{expressionString}'");
    }
}