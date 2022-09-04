namespace EngLang;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Parser;
using Yoakke.SynKit.Parser.Attributes;

[Parser(typeof(EngLangTokenType))]
public partial class EngLangParser
{
    private const string IdentifierReference = "identifier_reference";
    private const string LongIdentifier = "long_identifier";

    [Rule($"{LongIdentifier} : Identifier+")]
    private static string MakeLongIdentifier(IReadOnlyList<IToken<EngLangTokenType>> identifierParts)
    {
        return string.Join(' ', identifierParts.Select(_ => _.Text));
    }

    [Rule($"{IdentifierReference} : IndefiniteArticleKeyword {LongIdentifier}")]
    private static IdentifierReference MakeIdentifierReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        string identifier)
        => new IdentifierReference(identifier);

    [Rule($"variable_expression : {IdentifierReference}")]
    private static VariableExpression MakeVariableExpression(IdentifierReference e) => new (e);

    [Rule("primitive_expression : literal_expression")]
    [Rule($"primitive_expression : variable_expression")]
    private static Expression MakePrimitiveExpression(Expression e) => e;

    [Rule("math_expression : primitive_expression")]
    [Rule("math_expression : addition_expression")]
    private static Expression MakeMathExpression(Expression e) => e;

    [Rule("expression : math_expression")]
    [Rule("expression : assignment_expression")]
    [Rule("expression : inplace_addition_expression")]
    [Rule("expression : inplace_subtract_expression")]
    [Rule("expression : inplace_multiply_expression")]
    [Rule("expression : inplace_divide_expression")]
    private static Expression MakeExpression(Expression e) => e;

    [Rule($"addition_expression : primitive_expression 'plus' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'minus' primitive_expression")]
    private static MathExpression MakeAdditionExpression(
        Expression firstExpression,
        IToken<EngLangTokenType> mathToken,
        Expression secondExpression) => new(ToMathOperator(mathToken), firstExpression, secondExpression);

    [Rule($"addition_expression : primitive_expression 'multiply' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divide' 'by' primitive_expression")]
    private static MathExpression MakeAdditionExpression(
        Expression firstExpression,
        IToken<EngLangTokenType> mathToken,
        IToken<EngLangTokenType> byToken,
        Expression secondExpression) => new(ToMathOperator(mathToken), firstExpression, secondExpression);

    private static MathOperator ToMathOperator(IToken<EngLangTokenType> token) => token.Text switch
    {
        "plus" => MathOperator.Plus,
        "minus" => MathOperator.Minus,
        "multiply" => MathOperator.Multiply,
        "divide" => MathOperator.Divide,
        _ => throw new InvalidOperationException($"Unexpected token {token} for math expression"),
    };

    [Rule($"inplace_addition_expression : 'add' literal_expression 'to' {IdentifierReference}")]
    private static InPlaceAdditionExpression MakeInPlaceAdditionExpression(
        IToken<EngLangTokenType> addToken,
        Expression literalExpression,
        IToken<EngLangTokenType> toToken,
        IdentifierReference identifierReference) => new(literalExpression, identifierReference);

    [Rule($"inplace_subtract_expression : 'subtract' literal_expression 'from' {IdentifierReference}")]
    private static InPlaceSubtractExpression MakeInPlaceSubtractExpression(
        IToken<EngLangTokenType> subtractToken,
        Expression literalExpression,
        IToken<EngLangTokenType> fromToken,
        IdentifierReference identifierReference) => new(literalExpression, identifierReference);

    [Rule($"inplace_multiply_expression : 'multiply' {IdentifierReference} 'by' literal_expression")]
    private static InPlaceMultiplyExpression MakeInPlaceMultiplyExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression) => new(literalExpression, identifierReference);

    [Rule($"inplace_divide_expression : 'divide' {IdentifierReference} 'by' literal_expression")]
    private static InPlaceDivisionExpression MakeInPlaceDivisionExpression(
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

    [Rule($"variable_declaration: DefiniteArticleKeyword {LongIdentifier} 'is' {IdentifierReference} ('equal' 'to' literal_expression)?")]
    private static VariableDeclaration MakeVariableDeclaration(
        IToken<EngLangTokenType> definiteArticle,
        string identifier,
        IToken<EngLangTokenType> isToken,
        IdentifierReference identifierReference,
        (IToken<EngLangTokenType> equalToken,
        IToken<EngLangTokenType> toToken,
        Expression literalExpression)? x)
        => new VariableDeclaration(identifier, identifierReference, x?.literalExpression);

    [Rule($"assignment_expression: PutKeyword literal_expression 'into' {IdentifierReference}")]
    private static AssignmentExpression MakeAssignmentExpression(
        IToken<EngLangTokenType> putToken,
        Expression expression,
        IToken<EngLangTokenType> intoToken,
        IdentifierReference identifierReference)
        => new AssignmentExpression(identifierReference, expression);

    [Rule($"assignment_expression: 'let' {IdentifierReference} 'is' math_expression ")]
    private static AssignmentExpression MakeAlternateAssignmentExpression(
        IToken<EngLangTokenType> letToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        Expression expression)
        => new AssignmentExpression(identifierReference, expression);

    [Rule("simple_statement : expression_statement")]
    [Rule("simple_statement : variable_declaration_statement")]
    [Rule("simple_statement : if_statement")]
    private static Statement MakeSimpleStatement(
        Statement statement)
        => statement;

    [Rule("statement : simple_statement '.'")]
    [Rule("statement : block_statement '.'")]
    private static Statement MakeStatement(
        Statement statement,
        IToken<EngLangTokenType> dotToken)
        => statement;

    [Rule("statement_list : statement*")]
    private static BlockStatement MakeStatementList(
        IReadOnlyList<Statement> statements)
    {
        if (statements.Count == 1 && statements[0] is BlockStatement blockStatement)
        {
            return blockStatement;
        }

        return new BlockStatement(statements.ToImmutableList());
    }

    [Rule("variable_declaration_statement : variable_declaration")]
    private static VariableDeclarationStatement MakeVariableDeclarationStatement(
        VariableDeclaration declaration)
        => new VariableDeclarationStatement(declaration);

    [Rule("expression_statement : assignment_expression")]
    [Rule("expression_statement : math_expression")]
    [Rule("expression_statement : inplace_addition_expression")]
    [Rule("expression_statement : inplace_subtract_expression")]
    [Rule("expression_statement : inplace_multiply_expression")]
    [Rule("expression_statement : inplace_divide_expression")]
    private static ExpressionStatement MakeExpressionStatement(
        Expression expression)
        => new ExpressionStatement(expression);

    [Rule($"if_statement : 'if' {IdentifierReference} 'is' literal_expression 'then' expression_statement")]
    private static IfStatement MakeIfStatement(
        IToken<EngLangTokenType> ifToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        Expression literalExpression,
        IToken<EngLangTokenType> thenToken,
        Statement declaration)
        => new IfStatement(new EqualityExpression(identifierReference, literalExpression), declaration);

    [Rule("block_statement : (simple_statement (';' simple_statement)*)")]
    private static BlockStatement MakeBlockStatement(
        Punctuated<Statement, IToken<EngLangTokenType>> statements)
        => new BlockStatement(statements.Select(s => s.Value).ToImmutableList());

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
        var parser = new EngLangParser(new EngLangLexer(sourceCode));
        var blockStatmentResult = parser.ParseStatementList();
        if (blockStatmentResult.IsOk)
        {
            var variableReference = blockStatmentResult.Ok.Value;
            return variableReference;
        }

        throw new Exception($"Parser error. Got {blockStatmentResult.Error.Got} at position {blockStatmentResult.Error.Position}");
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
