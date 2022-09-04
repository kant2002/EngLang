using Yoakke.SynKit.Parser;

namespace EngLang;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Parser.Attributes;

[Parser(typeof(EngLangTokenType))]
public partial class EngLangParser
{
    //[Rule("statement : expression")]
    //private static Statement MakeExpressionStatement(Expression expression) => new ExpressionStatement(expression);

    [Rule("identifier_reference : IndefiniteArticleKeyword long_identifier")]
    private static IdentifierReference MakeIdentifierReference(
        IToken<EngLangTokenType> IndefiniteArticleKeyword,
        string identifier)
        => new IdentifierReference(identifier);

    [Rule("long_identifier : Identifier+")]
    private static string MakeLongIdentifier(
        IReadOnlyList<IToken<EngLangTokenType>> reminder
        //Punctuated<IToken<EngLangTokenType>, IToken<EngLangTokenType>> elements
    )
    {
        return string.Join(' ', reminder.Select(_ => _.Text));
    }

    [Rule("expression : literal_expression")]
    [Rule("expression : assignment_expression")]
    [Rule("expression : addition_expression")]
    [Rule("expression : subtract_expression")]
    [Rule("expression : multiply_expression")]
    [Rule("expression : divide_expression")]
    private static Expression MakeExpression(Expression e) => e;

    [Rule("addition_expression : 'add' literal_expression 'to' identifier_reference")]
    private static AdditionExpression MakeAdditionExpression(
        IToken<EngLangTokenType> addToken,
        Expression literalExpression,
        IToken<EngLangTokenType> toToken,
        IdentifierReference identifierReference) => new(literalExpression, identifierReference);

    [Rule("subtract_expression : 'subtract' literal_expression 'from' identifier_reference")]
    private static SubtractExpression MakeSubtractExpression(
        IToken<EngLangTokenType> subtractToken,
        Expression literalExpression,
        IToken<EngLangTokenType> fromToken,
        IdentifierReference identifierReference) => new(literalExpression, identifierReference);

    [Rule("multiply_expression : 'multiply' identifier_reference 'by' literal_expression")]
    private static MultiplyExpression MakeMultiplyExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression) => new(literalExpression, identifierReference);

    [Rule("divide_expression : 'divide' identifier_reference 'by' literal_expression")]
    private static DivisionExpression MakeDivisionExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression) => new(literalExpression, identifierReference);

    [Rule("literal_expression : StringLiteral")]
    [Rule("literal_expression : IntLiteral")]
    private static Expression MakeIdentifierReference(
        IToken<EngLangTokenType> token)
        => token.Kind switch
        {
            EngLangTokenType.StringLiteral => new StringLiteralExpression(token.Text.Trim('"')),
            EngLangTokenType.IntLiteral => new IntLiteralExpression(int.Parse(token.Text)),
            _ => throw new InvalidOperationException()
        };

    [Rule("variable_declaration: DefiniteArticleKeyword long_identifier 'is' identifier_reference ('equal' 'to' literal_expression)?")]
    private static VariableDeclaration MakeVariableDeclaration(
        IToken<EngLangTokenType> definiteArticle,
        string identifier,
        IToken<EngLangTokenType> isToken,
        IdentifierReference identifierReference,
        (IToken<EngLangTokenType> equalToken,
        IToken<EngLangTokenType> toToken,
        Expression literalExpression)? x)
        => new VariableDeclaration(identifier, identifierReference, x?.literalExpression);

    [Rule("assignment_expression: PutKeyword literal_expression 'into' identifier_reference")]
    private static AssignmentExpression MakeAssignmentExpression(
        IToken<EngLangTokenType> putToken,
        Expression expression,
        IToken<EngLangTokenType> intoToken,
        IdentifierReference identifierReference)
        => new AssignmentExpression(identifierReference, expression);

    public static SyntaxNode Parse(string sourceCode)
    {
        if (!sourceCode.Contains('.'))
        {
            // Fallback to expression parsing.
            return ParseNode(sourceCode);
        }

        return ParseBlockStatement(sourceCode);

    }

    private static BlockStatement ParseBlockStatement(string sourceCode)
    {
        var statementTexts = sourceCode.Split(new[] { '.', ';' }, StringSplitOptions.RemoveEmptyEntries);
        var statementsArray = statementTexts
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(ParseStatement)
            .ToArray();
        var statements = ImmutableList.Create<Statement>(statementsArray);
        return new BlockStatement(statements);

    }

    private static Statement ParseStatement(string content)
    {
        var expression = ParseNode(content);
        return ConvertToStatement(expression);
    }

    private static Statement ConvertToStatement(SyntaxNode node)
    {
        return node switch
        {
            VariableDeclaration variableDeclaration => new VariableDeclarationStatement(variableDeclaration),
            AssignmentExpression assignmentExpression => new AssignmentStatement(assignmentExpression),
            AdditionExpression additionExpression => new AdditionStatement(additionExpression),
            SubtractExpression subtractExpression => new SubtractStatement(subtractExpression),
            MultiplyExpression multiplyExpression => new MultiplyStatement(multiplyExpression),
            DivisionExpression divisionExpression => new DivisionStatement(divisionExpression),
            _ => throw new InvalidOperationException($"Node of type {node.GetType().Name} cannot be represented as statement"),
        };
    }

    private static SyntaxNode ParseNode(string sourceCode)
    {
        var parser = new EngLangParser(new EngLangLexer(sourceCode));
        var parts = sourceCode.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        switch (parts[0])
        {
            case "a":
            case "an":
                var variableReferenceResult = parser.ParseIdentifierReference();
                if (variableReferenceResult.IsOk)
                {
                    var variableReference = variableReferenceResult.Ok.Value;
                    return variableReference;
                }

                throw new Exception($"Parser error. Got {variableReferenceResult.Error.Got} at position {variableReferenceResult.Error.Position}");
            case "the":
                var variableDeclarationResult = parser.ParseVariableDeclaration();
                if (variableDeclarationResult.IsOk)
                {
                    var variableDeclaration = variableDeclarationResult.Ok.Value;
                    return variableDeclaration;
                }

                throw new Exception($"Parser error. Got {variableDeclarationResult.Error.Got} at position {variableDeclarationResult.Error.Position}");
            case "add":
            case "subtract":
            case "multiply":
            case "divide":
            case "put":
                var assignmentExpressionResult = parser.ParseExpression();
                if (assignmentExpressionResult.IsOk)
                {
                    return assignmentExpressionResult.Ok.Value;
                }

                throw new Exception($"Parser error. Got {assignmentExpressionResult.Error.Got} at position {assignmentExpressionResult.Error.Position}");
            default:
                throw new NotImplementedException($"Cannot parse expression starting from `{parts[0]}`");
        }
    }
}
