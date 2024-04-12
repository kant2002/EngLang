namespace EngLang;

using Humanizer;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Parser;
using Yoakke.SynKit.Parser.Attributes;

[Parser(typeof(EngLangTokenType))]
public partial class EngLangParser : IEngLangParser
{
    private const string IdentifierReference = "identifier_reference";
    private const string ParameterReference = "parameter_reference";
    private const string TypeIdentifierReference = "type_reference";
    private const string LongIdentifier = "long_identifier";
    private const string Identifier = "Identifier";

    [Rule($"{LongIdentifier} : {Identifier}+")]
    private static IReadOnlyList<string> MakeLongIdentifier(IReadOnlyList<IToken<EngLangTokenType>> identifierParts)
    {
        return identifierParts.Select(_ => _.Text).ToList();
    }

    [Rule($"{IdentifierReference} : IndefiniteArticleKeyword {LongIdentifier} ('of' {IdentifierReference})?")]
    private static IdentifierReference MakeIdentifierReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<string> identifiersList,
        (IToken<EngLangTokenType> ofToken,
        IdentifierReference parentReference)? parent)
    {
        StringBuilder currentIdentifier = new();
        IdentifierReference? parentReference = parent?.parentReference;
        bool nonFirst = false;
        foreach (var identifierPart in identifiersList)
        {
            if (!identifierPart.EndsWith("'s"))
            {
                if (nonFirst)
                {
                    currentIdentifier.Append(" ");
                }

                currentIdentifier.Append(identifierPart);
                nonFirst = true;
            }
            else
            {
                if (nonFirst)
                {
                    currentIdentifier.Append(" ");
                }

                currentIdentifier.Append(identifierPart[..(identifierPart.Length - 2)]);
                parentReference = new IdentifierReference(currentIdentifier.ToString(), parentReference);
                currentIdentifier = new();
            }
        }

        var identifier = currentIdentifier.ToString();
        return new IdentifierReference(identifier, parentReference);
    }

    [Rule($"{TypeIdentifierReference} : IndefiniteArticleKeyword {LongIdentifier}")]
    private static TypeIdentifierReference MakeTypeIdentifierReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<string> identifiersList)
    {
        return new TypeIdentifierReference(string.Join(" ", identifiersList), false);
    }

    [Rule($"{ParameterReference} : IndefiniteArticleKeyword {LongIdentifier}")]
    private static IdentifierReference MakeParameterReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<string> identifiersList)
    {
        return new IdentifierReference(string.Join(" ", identifiersList), null);
    }

    [Rule($"{ParameterReference} : 'some' {LongIdentifier}")]
    private static IdentifierReference MakeParameterArrayReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<string> identifiersList)
    {
        return new IdentifierReference(string.Join(" ", identifiersList), null);
    }

    [Rule($"{TypeIdentifierReference} : 'some' {LongIdentifier}")]
    private static TypeIdentifierReference MakeCollectionTypeIdentifierReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<string> identifiersList)
    {
        var typeName = string.Join(" ", identifiersList.SkipLast(1).Append(identifiersList.Last().Singularize()));
        return new TypeIdentifierReference(typeName, true);
    }

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
    [Rule("expression : logical_expression")]
    private static Expression MakeExpression(Expression e) => e;

    [Rule($"addition_expression : primitive_expression 'plus' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'minus' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'multiply' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'multiplied' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divide' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divided' primitive_expression")]
    private static MathExpression MakeAdditionExpression(
        Expression firstExpression,
        IToken<EngLangTokenType> mathToken,
        Expression secondExpression) => new(ToMathOperator(mathToken), firstExpression, secondExpression);

    [Rule($"addition_expression : primitive_expression 'multiply' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'multiplied' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divide' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divided' 'by' primitive_expression")]
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
        "multiplied" => MathOperator.Multiply,
        "divide" => MathOperator.Divide,
        "divided" => MathOperator.Divide,
        _ => throw new InvalidOperationException($"Unexpected token {token} for math expression"),
    };

    [Rule($"inplace_addition_expression : 'add' literal_expression 'to' {IdentifierReference}")]
    [Rule($"inplace_addition_expression : 'Add' literal_expression 'to' {IdentifierReference}")]
    private static InPlaceAdditionExpression MakeInPlaceAdditionExpression(
        IToken<EngLangTokenType> addToken,
        Expression literalExpression,
        IToken<EngLangTokenType> toToken,
        IdentifierReference identifierReference) => new(literalExpression, identifierReference);

    [Rule($"inplace_subtract_expression : 'subtract' literal_expression 'from' {IdentifierReference}")]
    [Rule($"inplace_subtract_expression : 'Subtract' literal_expression 'from' {IdentifierReference}")]
    private static InPlaceSubtractExpression MakeInPlaceSubtractExpression(
        IToken<EngLangTokenType> subtractToken,
        Expression literalExpression,
        IToken<EngLangTokenType> fromToken,
        IdentifierReference identifierReference) => new(literalExpression, identifierReference);

    [Rule($"inplace_multiply_expression : 'multiply' {IdentifierReference} 'by' primitive_expression")]
    [Rule($"inplace_multiply_expression : 'Multiply' {IdentifierReference} 'by' primitive_expression")]
    private static InPlaceMultiplyExpression MakeInPlaceMultiplyExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression) => new(literalExpression, identifierReference);

    [Rule($"inplace_divide_expression : 'divide' {IdentifierReference} 'by' primitive_expression")]
    [Rule($"inplace_divide_expression : 'Divide' {IdentifierReference} 'by' primitive_expression")]
    private static InPlaceDivisionExpression MakeInPlaceDivisionExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression) => new(literalExpression, identifierReference);

    [Rule("literal_expression : StringLiteral")]
    [Rule("literal_expression : IntLiteral")]
    [Rule("literal_expression : NullLiteral")]
    private static Expression MakeIdentifierReference(
        IToken<EngLangTokenType> token)
        => token.Kind switch
        {
            EngLangTokenType.StringLiteral => new StringLiteralExpression(token.Text.Trim('"')),
            EngLangTokenType.IntLiteral => new IntLiteralExpression(int.Parse(token.Text)),
            EngLangTokenType.NullLiteral => new NullLiteralExpression(),
            _ => throw new InvalidOperationException()
        };

    [Rule("constant_expression : ('+' | '-')? literal_expression")]
    private static Expression MakeConstantExpression(
        IToken<EngLangTokenType>? token,
        Expression expression)
    {
        Debug.Assert(token?.Text != "-", "negated expressions does not supported.");
        return expression;
    }

    [Rule($"variable_declaration: DefiniteArticleKeyword {LongIdentifier} 'is' {TypeIdentifierReference} (EqualKeyword 'to' constant_expression)?")]
    [Rule($"variable_declaration: DefiniteArticleKeyword {LongIdentifier} 'are' {TypeIdentifierReference} (EqualKeyword 'to' constant_expression)?")]
    private static VariableDeclaration MakeVariableDeclaration(
        IToken<EngLangTokenType> definiteArticle,
        IReadOnlyList<string> identifier,
        IToken<EngLangTokenType> isToken,
        TypeIdentifierReference typeReference,
        (IToken<EngLangTokenType> equalToken,
        IToken<EngLangTokenType> toToken,
        Expression literalExpression)? x)
        => new VariableDeclaration(string.Join(' ', identifier), typeReference, x?.literalExpression);

    [Rule($"shape_slot_list: (comma_identifier_references_list ('and' comma_identifier_references_list)*)")]
    private static IdentifierReferencesList MakeShapeSlotList(
        Punctuated<IdentifierReferencesList, IToken<EngLangTokenType>> slots)
        => new IdentifierReferencesList(slots.Values.SelectMany(_ => _.IdentifierReferences).ToImmutableList());

    [Rule($"shape_slot_list: comma_identifier_references_list ',' 'and' comma_identifier_references_list")]
    private static IdentifierReferencesList MakeShapeSlotList(
        IdentifierReferencesList first,
        IToken<EngLangTokenType> comma,
        IToken<EngLangTokenType> and,
        IdentifierReferencesList second)
        => new IdentifierReferencesList(first.IdentifierReferences.Union(second.IdentifierReferences).ToImmutableList());

    [Rule($"shape_declaration: IndefiniteArticleKeyword {LongIdentifier} 'is' {IdentifierReference} ('with' shape_slot_list)?")]
    private static ShapeDeclaration MakeShapeDeclaration(
        IToken<EngLangTokenType> indefiniteArticle,
        IReadOnlyList<string> identifier,
        IToken<EngLangTokenType> isToken,
        IdentifierReference identifierReference,
        (IToken<EngLangTokenType> withToken, IdentifierReferencesList slots)? slotsList)
        => new ShapeDeclaration(string.Join(' ', identifier), identifierReference, slotsList?.slots);

    [Rule($"shape_declaration: IndefiniteArticleKeyword {LongIdentifier} 'has' shape_slot_list")]
    private static ShapeDeclaration MakeShapeDeclaration(
        IToken<EngLangTokenType> indefiniteArticle,
        IReadOnlyList<string> identifier,
        IToken<EngLangTokenType> hasToken,
        IdentifierReferencesList slotsList)
        => new ShapeDeclaration(string.Join(' ', identifier), null, slotsList);

    [Rule($"assignment_expression: PutKeyword primitive_expression IntoKeyword {IdentifierReference}")]
    private static AssignmentExpression MakeAssignmentExpression(
        IToken<EngLangTokenType> putToken,
        Expression expression,
        IToken<EngLangTokenType> intoToken,
        IdentifierReference identifierReference)
        => new AssignmentExpression(identifierReference, expression);

    [Rule($"assignment_expression: 'let' {IdentifierReference} 'is' math_expression ")]
    [Rule($"assignment_expression: 'let' {IdentifierReference} EqualKeyword math_expression ")]
    private static AssignmentExpression MakeAlternateAssignmentExpression(
        IToken<EngLangTokenType> letToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        Expression expression)
        => new AssignmentExpression(identifierReference, expression);

    [Rule($"assignment_expression: 'let' {IdentifierReference} EqualKeyword 'to' math_expression ")]
    private static AssignmentExpression MakeAlternateAssignment2Expression(
        IToken<EngLangTokenType> letToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> equalsToken,
        IToken<EngLangTokenType> toToken,
        Expression expression)
        => new AssignmentExpression(identifierReference, expression);

    [Rule("expression_or_return_statement : expression_statement")]
    [Rule("expression_or_return_statement : result_statement")]
    [Rule("expression_or_return_statement : invocation_statement")]
    //[Rule("expression_or_return_statement : block_statement")]
    private static Statement MakeExpressionOrReturnStatement(
        Statement statement)
        => statement;

    [Rule("simple_statement : expression_or_return_statement")]
    [Rule("simple_statement : variable_declaration_statement")]
    [Rule("simple_statement : if_statement")]
    [Rule("simple_statement : shape_declaration_statement")]
    //[Rule("simple_statement : statementyy")]
    private static Statement MakeSimpleStatement(
        Statement statement)
        => statement;

    [Rule("statement : simple_statement '.'")]
    //[Rule("statement : labeled_statement_simple '.'")]
    [Rule("statement : block_statement '.'")]
    //[Rule("statement : invocation_statement '.'")]
    private static Statement MakeStatement(
        Statement statement,
        IToken<EngLangTokenType> dotToken)
        => statement;

    //[Rule("statement : labeled_statement")]
    //[Rule("statement : invalid_statement")]
    [Rule("statement : statementxx")]
    private static Statement MakeStatement(
        Statement statement)
        => statement;
    [Rule("statementxx : (Identifier|EqualKeyword|PutKeyword|LetKeyword|IfKeyword|IsKeyword|IntoKeyword|ByKeyword|AndKeyword|WithKeyword|OfKeyword|IntLiteral|StringLiteral|NullLiteral|ThenKeyword|IsKeyword|IndefiniteArticleKeyword|DefiniteArticleKeyword|MathOperationKeyword|LogicalOperationKeyword)* '.'")]
    private static Statement MakeStatement111(
        IEnumerable<IToken<EngLangTokenType>> tokens,
        IToken<EngLangTokenType> dotToken)
        => new InvalidStatement(tokens.ToImmutableArray());
    [Rule("statementyy : (Identifier|EqualKeyword|PutKeyword|LetKeyword|IfKeyword|IsKeyword|IntoKeyword|ByKeyword|AndKeyword|WithKeyword|OfKeyword|IntLiteral|StringLiteral|NullLiteral|ThenKeyword|IsKeyword|IndefiniteArticleKeyword|DefiniteArticleKeyword|MathOperationKeyword|LogicalOperationKeyword)*")]
    private static Statement MakeStatement222(
        IEnumerable<IToken<EngLangTokenType>> tokens)
        => new InvalidStatement(tokens.ToImmutableArray());

    [CustomParser("invalid_statement")]
    private ParseResult<EngLang.BlockStatement> parseInvalidStatement1(int offset)
    {
        ParseResult<EngLang.BlockStatement> a349;
        ParseResult<System.Collections.Generic.IReadOnlyList<EngLang.Statement>> a350;
        var a351 = new List<EngLang.Statement>();
        var currentOffset = offset;
        ParseError? a353 = null;
        while (true)
        {
            ParseResult<EngLang.Statement> parseStatementResult;
            parseStatementResult = parseStatement(currentOffset);
            if (parseStatementResult.IsError && (!this.TokenStream.TryLookAhead(currentOffset, out var a355) || ReferenceEquals(a355, parseStatementResult.Error.Got)))
            {
                parseStatementResult = ParseResult.Error("token", parseStatementResult.Error.Got, parseStatementResult.Error.Position, "statement_list");
            }

            if (parseStatementResult.IsError)
            {
                a353 = a353 | parseStatementResult.Error;
                break;
            }

            currentOffset = parseStatementResult.Ok.Offset;
            a351.Add(parseStatementResult.Ok.Value);
            a353 = a353 | parseStatementResult.Ok.FurthestError;
        }

        a350 = ParseResult.Ok((IReadOnlyList<EngLang.Statement>)a351, currentOffset, a353);
        if (a350.IsOk)
        {
            var a356 = a350.Ok.Value;
            a349 = ParseResult.Ok(MakeStatementList(a356), a350.Ok.Offset, a350.Ok.FurthestError);
        }
        else
        {
            a349 = a350.Error;
        }

        return a349;
    }

    [Rule("paragraph : statement_list")]
    private static Paragraph MakeParagraph(
        BlockStatement statement)
        => new Paragraph(statement.Statements, null);

    [Rule("paragraph_separator : Multiline+")]
    private static (bool, InvokableLabel?) MakeParagraphSeparator(
        IReadOnlyList<IToken<EngLangTokenType>> paragraphToken)
        => (true, null);

    [Rule("paragraph_separator : invokable_label")]
    private static (bool, InvokableLabel?) MakeParagraphSeparator(
        InvokableLabel invokableLabel)
        => (false, invokableLabel);

    [Rule("paragraph_separator : Multiline+ invokable_label")]
    private static (bool, InvokableLabel?) MakeParagraphSeparator(
        IReadOnlyList<IToken<EngLangTokenType>> paragraphToken,
        InvokableLabel invokableLabel)
        => (true, invokableLabel);

    // invokable_label
    [Rule("paragraph_list : Multiline* invokable_label? (paragraph (paragraph_separator paragraph)*)")]
    private static ParagraphList MakeParagraphList(
        IReadOnlyList<IToken<EngLangTokenType>> leadingMutlilines,
        InvokableLabel? firstInvokableLabel,
        Punctuated<Paragraph, (bool, InvokableLabel?)> statements)
    {
        InvokableLabel? currentLabel = firstInvokableLabel;
        var paragraphs = new List<Paragraph>();
        for (int i = 0; i < statements.Count; i++)
        {
            var paragraph = currentLabel is null
                ? statements[i].Value
                : statements[i].Value with { Label = currentLabel };
            currentLabel = statements[i].Punctuation.Item2;
            paragraphs.Add(paragraph);
        }

        return new ParagraphList(paragraphs.ToImmutableList());
    }

    [Rule("statement_list : statement+")]
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

    [Rule("shape_declaration_statement : shape_declaration")]
    private static ShapeDeclarationStatement MakeShapeDeclarationStatement(
        ShapeDeclaration declaration)
        => new ShapeDeclarationStatement(declaration);

    [Rule("expression_statement : assignment_expression")]
    [Rule("expression_statement : math_expression")]
    [Rule("expression_statement : inplace_addition_expression")]
    [Rule("expression_statement : inplace_subtract_expression")]
    [Rule("expression_statement : inplace_multiply_expression")]
    [Rule("expression_statement : inplace_divide_expression")]
    [Rule("expression_statement : logical_expression")]
    private static ExpressionStatement MakeExpressionStatement(
        Expression expression)
        => new ExpressionStatement(expression);

    [Rule($"logical_expression : {IdentifierReference} 'is' literal_expression")]
    private static LogicalExpression MakeLogicalExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        Expression literalExpression)
        => new LogicalExpression(LogicalOperator.Equals, new VariableExpression(identifierReference), literalExpression);

    [Rule($"logical_expression : {IdentifierReference} 'is' 'not' literal_expression")]
    private static LogicalExpression MakeNegativeLogicalExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        IToken<EngLangTokenType> notToken,
        Expression literalExpression)
        => new LogicalExpression(LogicalOperator.NotEquals, new VariableExpression(identifierReference), literalExpression);

    [Rule($"logical_expression : {IdentifierReference} LogicalOperationKeyword 'than' literal_expression")]
    private static LogicalExpression MakeLogicalExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> operatorToken,
        IToken<EngLangTokenType> thanToken,
        Expression literalExpression)
        => new LogicalExpression(GetLogicalOperator(operatorToken), new VariableExpression(identifierReference), literalExpression);

    private static LogicalOperator GetLogicalOperator(IToken<EngLangTokenType> operatorToken)
    {
        return operatorToken.Text switch
        {
            "less" => LogicalOperator.Less,
            "smaller" => LogicalOperator.Less,
            "greater" => LogicalOperator.Greater,
            "bigger" => LogicalOperator.Greater,
            _ => throw new InvalidOperationException($"Unknown logical operator {operatorToken.Text}"),
        };
    }

    [Rule($"if_statement : IfKeyword logical_expression ThenKeyword expression_or_return_statement")]
    [Rule($"if_statement : IfKeyword logical_expression ThenKeyword block_statement")]
    [Rule($"if_statement : IfKeyword logical_expression CommaKeyword expression_or_return_statement")]
    [Rule($"if_statement : IfKeyword logical_expression CommaKeyword block_statement")]
    private static IfStatement MakeIfEqualsStatement(
        IToken<EngLangTokenType> ifToken,
        LogicalExpression testExpression,
        IToken<EngLangTokenType> thenToken,
        Statement statement)
        => new IfStatement(testExpression, statement);

    [Rule("result_statement : 'result' 'is' math_expression")]
    private static ResultStatement MakeResultStatement(
        IToken<EngLangTokenType> resultToken,
        IToken<EngLangTokenType> isToken,
        Expression expression)
        => new ResultStatement(expression);

    [Rule("result_statement : 'the' 'result' 'is' math_expression")]
    private static ResultStatement MakeResultStatement(
        IToken<EngLangTokenType> theToken,
        IToken<EngLangTokenType> resultToken,
        IToken<EngLangTokenType> isToken,
        Expression expression)
        => new ResultStatement(expression);

    [Rule("result_statement : 'return' math_expression")]
    private static ResultStatement MakeResultStatement(
        IToken<EngLangTokenType> returnToken,
        Expression expression)
        => new ResultStatement(expression);

    [Rule("block_statement : (simple_statement (';' simple_statement)*)")]
    private static BlockStatement MakeBlockStatement(
        Punctuated<Statement, IToken<EngLangTokenType>> statements)
        => new BlockStatement(statements.Select(s => s.Value).ToImmutableList());

    [Rule($"identifier_references_list : ({IdentifierReference} 'and'?)*")]
    private static IdentifierReferencesList MakeIdentifierReferencesList(
        IReadOnlyList<(IdentifierReference, IToken<EngLangTokenType>?)> identifierReferences)
        => new IdentifierReferencesList(identifierReferences.Select(_ => _.Item1).ToImmutableList());

    [Rule($"parameter_references_list : ({ParameterReference} 'and'?)*")]
    private static IdentifierReferencesList MakeParameterReferencesList(
        IReadOnlyList<(IdentifierReference, IToken<EngLangTokenType>?)> identifierReferences)
        => new IdentifierReferencesList(identifierReferences.Select(_ => _.Item1).ToImmutableList());

    [Rule($"comma_identifier_references_list : ({IdentifierReference} (CommaKeyword {IdentifierReference})*)")]
    private static IdentifierReferencesList MakeCommaDelimitedIdentifierReferencesList(
        Punctuated<IdentifierReference, IToken<EngLangTokenType>> identifierReferences)
        => new IdentifierReferencesList(identifierReferences.Select(_ => _.Value).ToImmutableList());

    [Rule($"label_word : {Identifier}")]
    [Rule($"label_word : OfKeyword")]
    //[Rule($"label_word : IsKeyword")]
    //[Rule($"label_word : IfKeyword")]
    [Rule($"label_word : DefiniteArticleKeyword")]
    [Rule($"label_word : AndKeyword")]
    [Rule($"label_word : MathOperationKeyword")]
    [Rule($"label_word : LogicalOperationKeyword")]
    [Rule($"label_word : WithKeyword")]
    private static IToken<EngLangTokenType> MakeLabelWord(IToken<EngLangTokenType> marker)
        => marker;
    [Rule($"comment_label : '(' ({Identifier} | '-')* ')'")]
    private static string MakeInvokableLabel(
        IToken<EngLangTokenType> toToken,
        IReadOnlyList<IToken<EngLangTokenType>> names,
        IToken<EngLangTokenType> colonToken)
    {
        string labelName = string.Join(" ", names.Select(token => token.Text));
        return $"({labelName})";
    }

    [Rule($"invokable_label : 'to' label_word+ parameter_references_list comment_label? ':'")]
    [Rule($"invokable_label : 'to' label_word+ parameter_references_list comment_label? '->'")]
    [Rule($"invokable_label : 'To' label_word+ parameter_references_list comment_label? ':'")]
    [Rule($"invokable_label : 'To' label_word+ parameter_references_list comment_label? '->'")]
    [Rule($"invokable_label : 'define' label_word+ parameter_references_list comment_label? 'as'")]
    [Rule($"invokable_label : 'Define' label_word+ parameter_references_list comment_label? 'as'")]
    private static InvokableLabel MakeInvokableLabel(
        IToken<EngLangTokenType> toToken,
        IReadOnlyList<IToken<EngLangTokenType>> firstToken,
        IdentifierReferencesList identifierTokens,
        string? comment,
        IToken<EngLangTokenType> colonToken)
    {
        string labelName = string.Join(" ", firstToken.Select(i => i.Text));
        return new InvokableLabel(labelName + (comment is null ? "" : " " + comment), identifierTokens.IdentifierReferences.ToArray());
    }

    [Rule($"invokable_label : label_word+ parameter_references_list '->'")]
    private static InvokableLabel MakeInvokableLabel(
        IReadOnlyList<IToken<EngLangTokenType>> firstToken,
        IdentifierReferencesList identifierTokens,
        IToken<EngLangTokenType> asToken)
    {
        string labelName = string.Join(" ", firstToken.Select(i => i.Text));
        return new InvokableLabel(labelName, identifierTokens.IdentifierReferences.ToArray());
    }

    [Rule($"labeled_statement_simple : invokable_label block_statement")]
    private static LabeledStatement MakeSimpleLabeledStatement(
        InvokableLabel invokableLabel,
        Statement statement)
        => MakeLabeledStatement(invokableLabel, statement);

    [Rule($"labeled_statement : invokable_label paragraph")]
    private static LabeledStatement MakeLabeledStatement(
        InvokableLabel invokableLabel,
        Statement statement)
    {
        return new LabeledStatement(invokableLabel.Marker, invokableLabel.Parameters, statement);
    }

    [Rule($"invocation_statement : label_word+ identifier_references_list ('into' {IdentifierReference})?")]
    private static Statement MakeInvocationStatement(
        IReadOnlyList<IToken<EngLangTokenType>> firstToken,
        IdentifierReferencesList identifierTokens,
        (IToken<EngLangTokenType> intoToken,
        IdentifierReference outputIdentifier)? saveResultsGroup)
        => new InvocationStatement(string.Join(" ", firstToken.Select(i => i.Text)), identifierTokens.IdentifierReferences.ToArray(), saveResultsGroup?.outputIdentifier);

    public static SyntaxNode Parse(string sourceCode)
    {
        if (!sourceCode.Contains('.'))
        {
            // Fallback to expression parsing.
            return ParseNode(sourceCode);
        }

        return ParseParagraphList(sourceCode);

    }

    private static ParagraphList ParseParagraphList(string sourceCode)
    {
        var lexer = new EngLangLexer(sourceCode);
        var parser = new EngLangParser(lexer);
        var blockStatementResult = parser.ParseParagraphList();
        if (blockStatementResult.IsOk)
        {
            if (!lexer.IsEnd)
            {
                throw new Exception($"Parser error. Do not reach end of file. Currently at position {lexer.Position}, next lexema is {lexer.Next()}");
            }

            var blockStatement = blockStatementResult.Ok.Value;
            return blockStatement;
        }

        throw new Exception($"Parser error. Got {blockStatementResult.Error.Got} at position {blockStatementResult.Error.Position}");
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
