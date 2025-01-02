namespace EngLang;

using Humanizer;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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

    [Rule($"{LongIdentifier} : ({Identifier} | 'if')+")]
    private static IReadOnlyList<SymbolName> MakeLongIdentifier(IReadOnlyList<IToken<EngLangTokenType>> identifierParts)
    {
        return identifierParts.Select(_ => new SymbolName(_.Text, _.Range)).ToList();
    }

    [Rule($"{LongIdentifier} : 'A'")]
    private static IReadOnlyList<SymbolName> MakeA(IToken<EngLangTokenType> token)
    {
        return [new SymbolName(token.Text, token.Range)];
    }

    [Rule($"{LongIdentifier} : 'null' {Identifier}+")]
    [Rule($"{LongIdentifier} : LogicalOperationKeyword {Identifier}+")]
    private static IReadOnlyList<SymbolName> MakeNUllSomethingIdentifier(IToken<EngLangTokenType> token, IReadOnlyList<IToken<EngLangTokenType>> identifierParts)
    {
        return new[] { new SymbolName(token.Text, token.Range) }.Union(identifierParts.Select(_ => new SymbolName(_.Text, _.Range))).ToList();
    }

    [Rule($"{IdentifierReference} : IndefiniteArticleKeyword {LongIdentifier} ('of' {IdentifierReference})?")]
    [Rule($"{IdentifierReference} : DefiniteArticleKeyword {LongIdentifier} ('of' {IdentifierReference})?")]
    [Rule($"{IdentifierReference} : 'some' {LongIdentifier} ('of' {IdentifierReference})?")]
    private static IdentifierReference MakeIdentifierReference(
        IToken<EngLangTokenType> articleKeyword,
        IReadOnlyList<SymbolName> identifiersList,
        (IToken<EngLangTokenType> ofToken, IdentifierReference parentReference)? parent)
    {
        StringBuilder currentIdentifier = new();
        Yoakke.SynKit.Text.Position start = default;
        Yoakke.SynKit.Text.Position last = default;
        IdentifierReference? parentReference = parent?.parentReference;
        bool nonFirst = false;
        foreach (var identifierPart in identifiersList)
        {
            if (currentIdentifier.Length == 0)
            {
                start = identifierPart.Range.Start;
                last = identifierPart.Range.Start;
            }

            var possessive = identifierPart.Name.EndsWith("'s") || identifierPart.Name.EndsWith("'");
            if (!possessive)
            {
                if (nonFirst)
                {
                    currentIdentifier.Append(" ");
                }

                currentIdentifier.Append(identifierPart.Name);
                last = identifierPart.Range.End;
                nonFirst = true;
            }
            else
            {
                if (nonFirst)
                {
                    currentIdentifier.Append(" ");
                }

                var cleaned = identifierPart.Name.EndsWith("'") ? identifierPart.Name[..(identifierPart.Name.Length - 1)] : identifierPart.Name[..(identifierPart.Name.Length - 2)];
                currentIdentifier.Append(cleaned);
                last = identifierPart.Range.End;
                var reverseParentRange = parentReference is not null && parentReference.Range.End < start;
                var identifierName = new SymbolName(currentIdentifier.ToString(), new(start, last));
                parentReference = new IdentifierReference(
                    identifierName,
                    identifierName,
                    parentReference,
                    parentReference is null ? new(start, last) : reverseParentRange ? new(parentReference.Range.Start, last) : new(start, parentReference.Range.End));
                currentIdentifier = new();
                nonFirst = false;
            }
        }

        var identifier = currentIdentifier.ToString();
        var name = new SymbolName(identifier, new(start, last));
        return new IdentifierReference(
            name,
            name,
            parentReference,
            parentReference is null ? new(articleKeyword.Range.Start, last) : new(articleKeyword.Range.Start, last > parentReference.Range.End ? last : parentReference.Range.End));
    }

    [Rule($"{IdentifierReference} : {IdentifierReference} (NamedKeyword {LongIdentifier})")]
    private static IdentifierReference MakeNamedIdentifierReference(IdentifierReference identifier, (IToken<EngLangTokenType> token, IReadOnlyList<SymbolName> newName)? nameOverride)
    {
        if (nameOverride == null) return identifier;

        var newName = nameOverride.Value.newName;
        var identifierName = string.Join(" ", newName.Select(_ => _.Name));
        var lastRange = newName.Last().Range;
        return new IdentifierReference(
            new SymbolName(identifierName, new Yoakke.SynKit.Text.Range(newName.First().Range, lastRange)),
            identifier.Name,
            identifier.Owner,
            new Yoakke.SynKit.Text.Range(identifier.Range, lastRange));
    }

    [Rule($"{TypeIdentifierReference} : IndefiniteArticleKeyword {LongIdentifier}")]
    private static TypeIdentifierReference MakeTypeIdentifierReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<SymbolName> identifiersList)
    {
        Debug.Assert(identifiersList.Count != 0, "Identifier should be present");
        return new TypeIdentifierReference(string.Join(" ", identifiersList.Select(_ => _.Name)), false, new Yoakke.SynKit.Text.Range(indefiniteArticleKeyword.Range, identifiersList.Last().Range));
    }

    [Rule($"{ParameterReference} : IndefiniteArticleKeyword {LongIdentifier}")]
    private static IdentifierReference MakeParameterReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<SymbolName> identifiersList)
    {
        var symbol = new SymbolName(
            string.Join(" ", identifiersList.Select(_ => _.Name)),
            new (identifiersList.First().Range.Start, identifiersList.Last().Range.End));
        return new IdentifierReference(symbol, symbol, null, symbol.Range);
    }

    [Rule($"{ParameterReference} : 'some' {LongIdentifier}")]
    private static IdentifierReference MakeParameterArrayReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<SymbolName> identifiersList)
    {
        var symbol = new SymbolName(
            string.Join(" ", identifiersList.Select(_ => _.Name)),
            new(identifiersList.First().Range.Start, identifiersList.Last().Range.End));
        return new IdentifierReference(symbol, symbol, null, symbol.Range);
    }

    [Rule($"{ParameterReference} : {ParameterReference} (NamedKeyword {LongIdentifier})")]
    private static IdentifierReference MakeNamedParameterReference(IdentifierReference identifier, (IToken<EngLangTokenType> token, IReadOnlyList<SymbolName> newName)? nameOverride)
    {
        if (nameOverride == null) return identifier;

        var newName = nameOverride.Value.newName;
        var identifierName = string.Join(" ", newName.Select(_ => _.Name));
        var lastRange = newName.Last().Range;
        return new IdentifierReference(
            new SymbolName(identifierName, new Yoakke.SynKit.Text.Range(newName.First().Range, lastRange)),
            identifier.Name,
            identifier.Owner,
            new Yoakke.SynKit.Text.Range(identifier.Range, lastRange));
    }

    [Rule($"{TypeIdentifierReference} : 'some' {LongIdentifier}")]
    private static TypeIdentifierReference MakeCollectionTypeIdentifierReference(
        IToken<EngLangTokenType> indefiniteArticleKeyword,
        IReadOnlyList<SymbolName> identifiersList)
    {
        var typeName = string.Join(" ", identifiersList.SkipLast(1).Select(_ => _.Name).Append(identifiersList[identifiersList.Count - 1].Name.Singularize()));
        return new TypeIdentifierReference(typeName, true, new Yoakke.SynKit.Text.Range(indefiniteArticleKeyword.Range, identifiersList.Last().Range));
    }

    [Rule($"variable_expression : {IdentifierReference}")]
    private static VariableExpression MakeVariableExpression(IdentifierReference e) => new (e, e.Range);

    [Rule($"posessive_expression : literal_expression PosessiveKeyword {LongIdentifier}")]
    private static PosessiveExpression MakePosessiveExpression(Expression target, IToken<EngLangTokenType> posessiveKeyword, IReadOnlyList<SymbolName> names)
    {
        var symbol = new SymbolName(
            string.Join(" ", names.Select(_ => _.Name)),
            new(names.First().Range.Start, names.Last().Range.End));
        IdentifierReference identifier = new IdentifierReference(symbol, symbol, null, symbol.Range);
        var range = new Yoakke.SynKit.Text.Range(target.Range, names.Last().Range);
        return new PosessiveExpression(identifier, target, range);
    }

    [Rule("primitive_expression : constant_expression")]
    [Rule($"primitive_expression : variable_expression")]
    [Rule($"primitive_expression : posessive_expression")]
    private static Expression MakePrimitiveExpression(Expression e) => e;

    [Rule("math_expression : primitive_expression")]
    [Rule("math_expression : addition_expression")]
    private static Expression MakeMathExpression(Expression e) => e;

    [Rule("inplace_expression : inplace_addition_expression")]
    [Rule("inplace_expression : inplace_subtract_expression")]
    [Rule("inplace_expression : inplace_multiply_expression")]
    [Rule("inplace_expression : inplace_divide_expression")]
    private static Expression MakeInplaceExpression(Expression e) => e;

    [Rule("expression : math_expression")]
    [Rule("expression : assignment_expression")]
    [Rule("expression : inplace_expression")]
    [Rule("expression : logical_expression")]
    private static Expression MakeExpression(Expression e) => e;

    [Rule($"addition_expression : primitive_expression 'plus' primitive_expression")]
    [Rule($"addition_expression : primitive_expression '+' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'minus' primitive_expression")]
    [Rule($"addition_expression : primitive_expression '-' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'multiply' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'times' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'multiplied' primitive_expression")]
    [Rule($"addition_expression : primitive_expression '*' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divide' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divided' primitive_expression")]
    [Rule($"addition_expression : primitive_expression '/' primitive_expression")]
    private static MathExpression MakeAdditionExpression(
        Expression firstExpression,
        IToken<EngLangTokenType> mathToken,
        Expression secondExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(firstExpression.Range, secondExpression.Range);
        return new(ToMathOperator(mathToken), firstExpression, secondExpression, range);
    }

    [Rule($"addition_expression : primitive_expression 'multiply' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'multiplied' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divide' 'by' primitive_expression")]
    [Rule($"addition_expression : primitive_expression 'divided' 'by' primitive_expression")]
    private static MathExpression MakeAdditionExpression(
        Expression firstExpression,
        IToken<EngLangTokenType> mathToken,
        IToken<EngLangTokenType> byToken,
        Expression secondExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(firstExpression.Range, secondExpression.Range);
        return new(ToMathOperator(mathToken), firstExpression, secondExpression, range);
    }

    private static MathOperator ToMathOperator(IToken<EngLangTokenType> token) => token.Text switch
    {
        "plus" => MathOperator.Plus,
        "+" => MathOperator.Plus,
        "minus" => MathOperator.Minus,
        "-" => MathOperator.Minus,
        "multiply" => MathOperator.Multiply,
        "multiplied" => MathOperator.Multiply,
        "times" => MathOperator.Multiply,
        "*" => MathOperator.Multiply,
        "divide" => MathOperator.Divide,
        "divided" => MathOperator.Divide,
        "/" => MathOperator.Divide,
        _ => throw new InvalidOperationException($"Unexpected token {token} for math expression"),
    };

    [Rule($"inplace_addition_expression : 'add' math_expression 'to' {IdentifierReference}")]
    [Rule($"inplace_addition_expression : 'Add' math_expression 'to' {IdentifierReference}")]
    private static InPlaceAdditionExpression MakeInPlaceAdditionExpression(
        IToken<EngLangTokenType> addToken,
        Expression literalExpression,
        IToken<EngLangTokenType> toToken,
        IdentifierReference identifierReference)
    {
        var range = new Yoakke.SynKit.Text.Range(addToken.Range, identifierReference.Range);
        return new(literalExpression, identifierReference, range);
    }

    [Rule($"inplace_subtract_expression : 'subtract' math_expression 'from' {IdentifierReference}")]
    [Rule($"inplace_subtract_expression : 'Subtract' math_expression 'from' {IdentifierReference}")]
    private static InPlaceSubtractExpression MakeInPlaceSubtractExpression(
        IToken<EngLangTokenType> subtractToken,
        Expression literalExpression,
        IToken<EngLangTokenType> fromToken,
        IdentifierReference identifierReference)
    {
        var range = new Yoakke.SynKit.Text.Range(subtractToken.Range, identifierReference.Range);
        return new(literalExpression, identifierReference, range);
    }

    [Rule($"inplace_multiply_expression : 'multiply' {IdentifierReference} 'by' math_expression")]
    [Rule($"inplace_multiply_expression : 'Multiply' {IdentifierReference} 'by' math_expression")]
    private static InPlaceMultiplyExpression MakeInPlaceMultiplyExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(multiplyToken.Range, literalExpression.Range);
        return new(literalExpression, identifierReference, range);
    }

    [Rule($"inplace_divide_expression : 'divide' {IdentifierReference} 'by' math_expression")]
    [Rule($"inplace_divide_expression : 'Divide' {IdentifierReference} 'by' math_expression")]
    private static InPlaceDivisionExpression MakeInPlaceDivisionExpression(
        IToken<EngLangTokenType> multiplyToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> byToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(multiplyToken.Range, literalExpression.Range);
        return new(literalExpression, identifierReference, range);
    }

    [Rule("literal_expression : StringLiteral")]
    [Rule("literal_expression : IntLiteral")]
    [Rule("literal_expression : NullLiteral")]
    [Rule("literal_expression : HexLiteral")]
    [Rule("literal_expression : RatioLiteral")]
    private static Expression MakeIdentifierReference(
        IToken<EngLangTokenType> token)
        => token.Kind switch
        {
            EngLangTokenType.StringLiteral => new StringLiteralExpression(token.Text[1..(token.Text.Length - 1)].Replace("\"\"", "\""), token.Range),
            EngLangTokenType.IntLiteral => new IntLiteralExpression(long.Parse(token.Text), token.Range),
            EngLangTokenType.NullLiteral => new NullLiteralExpression(token.Range),
            EngLangTokenType.RatioLiteral => new RatioLiteralExpression(int.Parse(token.Text.Split('/')[0]), int.Parse(token.Text.Split('/')[1]), token.Range),
            EngLangTokenType.HexLiteral => new ByteArrayLiteralExpression(ConvertHexToByteArray(token.Text[0] == '$' ? token.Text[1..] : token.Text[2..]), token.Range),
            _ => throw new InvalidOperationException()
        };

    [Rule("literal_expression : IntLiteral 'inch'")]
    private static Expression MakeInchIdentifierReference(
        IToken<EngLangTokenType> token,
        IToken<EngLangTokenType> inchToken)
        => token.Kind switch
        {
            EngLangTokenType.IntLiteral => new InchLiteralExpression(int.Parse(token.Text), token.Range),
            _ => throw new InvalidOperationException()
        };

    private static byte[] ConvertHexToByteArray(string hexString)
    {
        return Enumerable.Range(0, hexString.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                         .ToArray();
    }

    [Rule("constant_expression : ('+' | '-')? literal_expression")]
    private static Expression MakeConstantExpression(
        IToken<EngLangTokenType>? token,
        Expression expression)
    {
        if (token?.Text == "-")
        {
            if (expression is IntLiteralExpression intLiteralExpression)
            {
                return new IntLiteralExpression(-intLiteralExpression.Value, new Yoakke.SynKit.Text.Range(token.Range, expression.Range));
            }
            if (expression is InchLiteralExpression inchLiteralExpression)
            {
                return new InchLiteralExpression(-inchLiteralExpression.Value, new Yoakke.SynKit.Text.Range(token.Range, expression.Range));
            }
            if (expression is RatioLiteralExpression ratioLiteralExpression)
            {
                return new RatioLiteralExpression(-ratioLiteralExpression.Numerator, ratioLiteralExpression.Denominator, new Yoakke.SynKit.Text.Range(token.Range, expression.Range));
            }
        }

        Debug.Assert(token?.Text != "-", "negated expressions does not supported.");
        return expression;
    }

    [Rule($"variable_declaration: (DefiniteArticleKeyword|SomeKeyword) {LongIdentifier} ('is'|'are') {TypeIdentifierReference} (EqualKeyword 'to' constant_expression)?")]
    private static VariableDeclaration MakeVariableDeclaration(
        IToken<EngLangTokenType> definiteArticle,
        IReadOnlyList<SymbolName> identifier,
        IToken<EngLangTokenType> isToken,
        TypeIdentifierReference typeReference,
        (IToken<EngLangTokenType> equalToken, IToken<EngLangTokenType> toToken, Expression literalExpression)? x)
    {
        var range = new Yoakke.SynKit.Text.Range(definiteArticle.Range, x?.literalExpression.Range ?? typeReference.Range);
        return new VariableDeclaration(string.Join(' ', identifier.Select(_ => _.Name)), typeReference, x?.literalExpression, range);
    }

    [Rule($"shape_slot_list: (comma_identifier_references_list ('and' comma_identifier_references_list)*)")]
    private static SlotDeclarationsList MakeShapeSlotList(
        Punctuated<SlotDeclarationsList, IToken<EngLangTokenType>> slots)
        => new SlotDeclarationsList(slots.Values.SelectMany(_ => _.Slots).ToImmutableList());

    [Rule($"shape_slot_list: comma_identifier_references_list ',' 'and' comma_identifier_references_list")]
    private static SlotDeclarationsList MakeShapeSlotList(
        SlotDeclarationsList first,
        IToken<EngLangTokenType> comma,
        IToken<EngLangTokenType> and,
        SlotDeclarationsList second)
        => new SlotDeclarationsList(first.Slots.Union(second.Slots).ToImmutableList());

    [Rule($"shape_declaration: IndefiniteArticleKeyword {LongIdentifier} 'is' {TypeIdentifierReference} ('with' shape_slot_list)?")]
    [Rule($"shape_declaration: SomeKeyword {LongIdentifier} 'is' {TypeIdentifierReference} ('with' shape_slot_list)?")]
    private static ShapeDeclaration MakeShapeDeclaration(
        IToken<EngLangTokenType> indefiniteArticle,
        IReadOnlyList<SymbolName> identifier,
        IToken<EngLangTokenType> isToken,
        TypeIdentifierReference identifierReference,
        (IToken<EngLangTokenType> withToken, SlotDeclarationsList slots)? slotsList)
    {
        var symbol = new SymbolName(
            string.Join(" ", identifier.Select(_ => _.Name)),
            new(identifier.First().Range.Start, identifier.Last().Range.End));
        var range = new Yoakke.SynKit.Text.Range(indefiniteArticle.Range, slotsList is null ? identifierReference.Range : slotsList.Value.slots.Slots.Last().Range);
        return new ShapeDeclaration(symbol, identifierReference, slotsList?.slots, range);
    }

    [Rule($"shape_declaration: IndefiniteArticleKeyword {LongIdentifier} 'has' shape_slot_list")]
    private static ShapeDeclaration MakeShapeDeclaration(
        IToken<EngLangTokenType> indefiniteArticle,
        IReadOnlyList<SymbolName> identifier,
        IToken<EngLangTokenType> hasToken,
        SlotDeclarationsList slotsList)
    {
        var symbol = new SymbolName(
            string.Join(" ", identifier.Select(_ => _.Name)),
            new(identifier[0].Range.Start, identifier[identifier.Count - 1].Range.End));
        return new ShapeDeclaration(symbol, null, slotsList, new Yoakke.SynKit.Text.Range(indefiniteArticle.Range, slotsList.Slots.Last().Range));
    }

    [Rule($"assignment_expression: PutKeyword math_expression IntoKeyword {IdentifierReference}")]
    private static AssignmentExpression MakeAssignmentExpression(
        IToken<EngLangTokenType> putToken,
        Expression expression,
        IToken<EngLangTokenType> intoToken,
        IdentifierReference identifierReference)
    {
        var range = new Yoakke.SynKit.Text.Range(putToken.Range, identifierReference.Range);
        return new AssignmentExpression(identifierReference, expression, range);
    }

    //[Rule($"assignment_expression: inplace_expression ResultingKeyword {IdentifierReference}")]
    [Rule($"assignment_expression: inplace_expression IntoKeyword {IdentifierReference}")]
    private static AssignmentExpression MakeAssignmentExpressionResulting(
        Expression expression,
        IToken<EngLangTokenType> intoToken,
        IdentifierReference identifierReference)
    {
        var range = new Yoakke.SynKit.Text.Range(expression.Range, identifierReference.Range);
        return new AssignmentExpression(identifierReference, expression, range);
    }

    [Rule($"assignment_expression: 'let' {IdentifierReference} 'is' math_expression ")]
    [Rule($"assignment_expression: 'let' {IdentifierReference} EqualKeyword math_expression ")]
    private static AssignmentExpression MakeAlternateAssignmentExpression(
        IToken<EngLangTokenType> letToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        Expression expression)
    {
        var range = new Yoakke.SynKit.Text.Range(letToken.Range, expression.Range);
        return new AssignmentExpression(identifierReference, expression, range);
    }

    [Rule($"assignment_expression: 'let' {IdentifierReference} EqualKeyword 'to' math_expression ")]
    private static AssignmentExpression MakeAlternateAssignment2Expression(
        IToken<EngLangTokenType> letToken,
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> equalsToken,
        IToken<EngLangTokenType> toToken,
        Expression expression)
    {
        var range = new Yoakke.SynKit.Text.Range(letToken.Range, expression.Range);
        return new AssignmentExpression(identifierReference, expression, range);
    }

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
    [Rule("simple_statement : statementyy")]
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
    [Rule("statementxx : (Identifier|EqualKeyword|PutKeyword|LetKeyword|IfKeyword|IsKeyword|IntoKeyword|ByKeyword|AndKeyword|WithKeyword|OfKeyword|IntLiteral|RatioLiteral|StringLiteral|NullLiteral|HexLiteral|ThenKeyword|IsKeyword|HasKeyword|IndefiniteArticleKeyword|DefiniteArticleKeyword|FunctionBodyOrAsKeyword|MathOperationKeyword|LogicalOperationKeyword|OnKeyword|SomeKeyword|AtKeyword|FromKeyword|ToKeyword|PosessiveKeyword|'/'|'('|')')* '.'")]
    private static Statement MakeStatement111(
        IEnumerable<IToken<EngLangTokenType>> tokens,
        IToken<EngLangTokenType> dotToken)
    {
        var statementTokens = tokens.Union([dotToken]).ToImmutableArray();
        return new InvalidStatement(statementTokens, new Yoakke.SynKit.Text.Range(statementTokens.First().Range, statementTokens.Last().Range));
    }

    [Rule("statementyy : (Identifier|EqualKeyword|PutKeyword|LetKeyword|IfKeyword|IsKeyword|IntoKeyword|ByKeyword|AndKeyword|WithKeyword|OfKeyword|IntLiteral|RatioLiteral|StringLiteral|NullLiteral|HexLiteral|ThenKeyword|IsKeyword|HasKeyword|IndefiniteArticleKeyword|DefiniteArticleKeyword|FunctionBodyOrAsKeyword|MathOperationKeyword|LogicalOperationKeyword|FromKeyword|ToKeyword|PosessiveKeyword)*")]
    private static Statement MakeStatement222(
        IEnumerable<IToken<EngLangTokenType>> tokens)
    {
        var tkns = tokens.ToImmutableArray();
        var range = tkns.IsEmpty ? default : new Yoakke.SynKit.Text.Range(tokens.First().Range, tokens.Last().Range);
        return new InvalidStatement(tkns, range);
    }

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
        => new Paragraph(statement.Statements, null, statement.Range);

    [Rule("paragraph_list_element : invokable_label paragraph?")]
    private static Paragraph MakeParagraphListElement(InvokableLabel il, Paragraph p)
    {
        var range = p == null ? il.Range : new Yoakke.SynKit.Text.Range(il.Range, p.Range);
        return new Paragraph(p?.Statements ?? ImmutableList<Statement>.Empty, il, range);
    }

    [Rule("paragraph_list_element : paragraph")]
    private static Paragraph MakeParagraphListElement(Paragraph p)
    {
        return p;
    }

    [Rule("paragraph_list : Multiline* (paragraph_list_element (Multiline* paragraph_list_element)*)? Multiline*")]
    private static ParagraphList MakeParagraphList(IReadOnlyList<IToken<EngLangTokenType>> leading, Punctuated<Paragraph, IReadOnlyList<IToken<EngLangTokenType>>> para, IReadOnlyList<IToken<EngLangTokenType>> trailing)
    {
        return new ParagraphList(para.Values.ToImmutableList());
    }

    [Rule("statement_list : statement+")]
    private static BlockStatement MakeStatementList(
        IReadOnlyList<Statement> statements)
    {
        if (statements.Count == 1 && statements[0] is BlockStatement blockStatement)
        {
            return blockStatement;
        }

        var validStatements = statements.Where(_ => _ is not InvalidStatement invalidStatement || invalidStatement.Tokens.Length > 0).ToImmutableList();
        return new BlockStatement(validStatements, new Yoakke.SynKit.Text.Range(validStatements[0].Range, validStatements.Last().Range));
    }

    [Rule("variable_declaration_statement : variable_declaration")]
    private static VariableDeclarationStatement MakeVariableDeclarationStatement(
        VariableDeclaration declaration)
        => new VariableDeclarationStatement(declaration, declaration.Range);

    [Rule("shape_declaration_statement : shape_declaration")]
    private static ShapeDeclarationStatement MakeShapeDeclarationStatement(
        ShapeDeclaration declaration)
        => new ShapeDeclarationStatement(declaration, declaration.Range);

    [Rule("expression_statement : assignment_expression")]
    [Rule("expression_statement : math_expression")]
    [Rule("expression_statement : inplace_addition_expression")]
    [Rule("expression_statement : inplace_subtract_expression")]
    [Rule("expression_statement : inplace_multiply_expression")]
    [Rule("expression_statement : inplace_divide_expression")]
    //[Rule("expression_statement : logical_expression")]
    private static ExpressionStatement MakeExpressionStatement(
        Expression expression)
        => new ExpressionStatement(expression, expression.Range);

    [Rule($"logical_expression : {IdentifierReference} ('is'|'are') primitive_expression")]
    private static LogicalExpression MakeLogicalExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        return new LogicalExpression(LogicalOperator.Equals, new VariableExpression(identifierReference, identifierReference.Range), literalExpression, range);
    }

    [Rule($"logical_expression : {IdentifierReference} ('is'|'are') 'not' constant_expression")]
    private static LogicalExpression MakeNegativeLogicalExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        IToken<EngLangTokenType> notToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        return new LogicalExpression(LogicalOperator.NotEquals, new VariableExpression(identifierReference, identifierReference.Range), literalExpression, range);
    }

    [Rule($"logical_expression : {IdentifierReference} LogicalOperationKeyword 'than' primitive_expression")]
    private static LogicalExpression MakeLogicalThanExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> operatorToken,
        IToken<EngLangTokenType> thanToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        var varExpression = new VariableExpression(identifierReference, identifierReference.Range);
        return new LogicalExpression(GetLogicalOperator(operatorToken), varExpression, literalExpression, range);
    }

    [Rule($"logical_expression : {IdentifierReference} ('is'|'are') LogicalOperationKeyword 'than' primitive_expression")]
    private static LogicalExpression MakeLogicalThanExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        IToken<EngLangTokenType> operatorToken,
        IToken<EngLangTokenType> thanToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        var varExpression = new VariableExpression(identifierReference, identifierReference.Range);
        return new LogicalExpression(GetLogicalOperator(operatorToken), varExpression, literalExpression, range);
    }

    [Rule($"logical_expression : {IdentifierReference} 'at' LogicalOperationKeyword primitive_expression")]
    private static LogicalExpression MakeLogicalAtExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> atToken,
        IToken<EngLangTokenType> operatorToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        var varExpression = new VariableExpression(identifierReference, identifierReference.Range);
        return new LogicalExpression(GetLogicalOperator(operatorToken), varExpression, literalExpression, range);
    }

    [Rule($"logical_expression : {IdentifierReference} ('is'|'are') 'at' LogicalOperationKeyword primitive_expression")]
    private static LogicalExpression MakeLogicalIsAtExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        IToken<EngLangTokenType> atToken,
        IToken<EngLangTokenType> operatorToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        var varExpression = new VariableExpression(identifierReference, identifierReference.Range);
        return new LogicalExpression(GetLogicalOperator(operatorToken), varExpression, literalExpression, range);
    }

    [Rule($"logical_expression : {IdentifierReference} ('is'|'are')? LogicalOperationKeyword 'than' 'or' 'equal' 'to'? primitive_expression ")]
    private static LogicalExpression MakeLogicalThanExpression(
        IdentifierReference identifierReference,
        IToken<EngLangTokenType> isToken,
        IToken<EngLangTokenType> operatorToken,
        IToken<EngLangTokenType> thanToken,
        IToken<EngLangTokenType> orToken,
        IToken<EngLangTokenType> equalToken,
        IToken<EngLangTokenType>? toToken,
        Expression literalExpression)
    {
        var range = new Yoakke.SynKit.Text.Range(identifierReference.Range, literalExpression.Range);
        var varExpression = new VariableExpression(identifierReference, identifierReference.Range);
        return new LogicalExpression(GetLogicalOperator(operatorToken), varExpression, literalExpression, range);
    }

    [Rule($"logical_expression : (IndefiniteArticleKeyword|DefiniteArticleKeyword|IsKeyword|AtKeyword|OnKeyword|WithKeyword|'of'|{Identifier}|IntoKeyword|AndKeyword|IntLiteral|RatioLiteral|HexLiteral|StringLiteral|'/'|AreKeyword|FunctionBodyOrAsKeyword|LogicalOperationKeyword|MathOperationKeyword|ByKeyword|NullLiteral|FromKeyword|ToKeyword|')'|'(')*")]
    private static LogicalExpression MakeInvalidLogicalExpression(
        IReadOnlyList<IToken<EngLangTokenType>> someTokens)
    {
        var range = someTokens.Count == 0 ? default : new Yoakke.SynKit.Text.Range(someTokens.First().Range, someTokens.Last().Range);
        return new InvalidExpression(string.Join(" ", someTokens.Select(_ => _.Text)), range);
    }

    private static LogicalOperator GetLogicalOperator(IToken<EngLangTokenType> operatorToken)
    {
        return operatorToken.Text switch
        {
            "less" => LogicalOperator.Less,
            "smaller" => LogicalOperator.Less,
            "greater" => LogicalOperator.Greater,
            "bigger" => LogicalOperator.Greater,
            "least" => LogicalOperator.GreaterOrEquals,
            "most" => LogicalOperator.LessOrEquals,
            _ => throw new InvalidOperationException($"Unknown logical operator {operatorToken.Text}"),
        };
    }

    private static LogicalOperator GetLogicalOperatorOrEqual(LogicalOperator operatorToken)
    {
        return operatorToken switch
        {
            LogicalOperator.Less => LogicalOperator.LessOrEquals,
            LogicalOperator.Greater => LogicalOperator.GreaterOrEquals,
            _ => throw new InvalidOperationException($"Logical operator {operatorToken} cannot be converted to 'or equals' form"),
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
        => new IfStatement(testExpression, statement, new Yoakke.SynKit.Text.Range(ifToken.Range, statement.Range));

    [Rule("result_statement : 'result' 'is' math_expression")]
    private static ResultStatement MakeResultStatement(
        IToken<EngLangTokenType> resultToken,
        IToken<EngLangTokenType> isToken,
        Expression expression)
        => new ResultStatement(expression, new Yoakke.SynKit.Text.Range(resultToken.Range, expression.Range));

    [Rule("result_statement : 'the' 'result' 'is' math_expression")]
    private static ResultStatement MakeResultStatement(
        IToken<EngLangTokenType> theToken,
        IToken<EngLangTokenType> resultToken,
        IToken<EngLangTokenType> isToken,
        Expression expression)
        => new ResultStatement(expression, new Yoakke.SynKit.Text.Range(resultToken.Range, expression.Range));

    [Rule("result_statement : 'return' math_expression")]
    private static ResultStatement MakeResultStatement(
        IToken<EngLangTokenType> returnToken,
        Expression expression)
        => new ResultStatement(expression, new Yoakke.SynKit.Text.Range(returnToken.Range, expression.Range));

    [Rule("block_statement : (simple_statement (';' simple_statement)*)")]
    private static BlockStatement MakeBlockStatement(
        Punctuated<Statement, IToken<EngLangTokenType>> statements)
    {
        ImmutableList<Statement> stmts = statements.Select(s => s.Value).ToImmutableList();
        return new BlockStatement(stmts, new Yoakke.SynKit.Text.Range(stmts.First().Range, stmts.Last().Range));
    }

    [Rule($"parameter_expression : {IdentifierReference}")]
    [Rule($"parameter_expression : literal_expression")]
    public Expression MakeParameterExpression(SyntaxNode node)
    {
        return node switch
        {
            IdentifierReference ir => new VariableExpression(ir, ir.Range),
            IntLiteralExpression expr => expr,
            RatioLiteralExpression expr => expr,
            NullLiteralExpression expr => expr,
            InchLiteralExpression expr => expr,
            StringLiteralExpression expr => expr,
            ByteArrayLiteralExpression expr => expr,
            _ => throw new InvalidOperationException(),
        };
    }

    [Rule($"identifier_references_list : (primitive_expression extended_label_word?)*")]
    private static (IEnumerable<IToken<EngLangTokenType>> InnerText, ExpressionList Identifiers) MakeIdentifierReferencesList(
        IReadOnlyList<(Expression, IToken<EngLangTokenType>?)> identifierReferences)
        => (identifierReferences.Where(_ => _.Item2 is not null).Select(_ => _.Item2!), new ExpressionList(identifierReferences.Select(_ => _.Item1).ToImmutableList<Expression>()));

    [Rule($"parameter_references_list : ({ParameterReference} extended_def_label_word*)*")]
    private static (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) MakeParameterReferencesList(
        IReadOnlyList<(IdentifierReference, IReadOnlyList<IToken<EngLangTokenType>>)> identifierReferences)
        => (identifierReferences.SelectMany(_ => _.Item2).ToImmutableList(), identifierReferences.Select(_ => _.Item1).ToImmutableList());

    [Rule($"parameter_references_list_strict : ({ParameterReference} extended_def_label_word_strict*)*")]
    private static (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) MakeParameterReferencesListStrict(
        IReadOnlyList<(IdentifierReference, IReadOnlyList<IToken<EngLangTokenType>>)> identifierReferences)
        => MakeParameterReferencesList(identifierReferences);

    [Rule($"slot_declaration : {TypeIdentifierReference} (NamedKeyword {LongIdentifier})? ('is' {IdentifierReference})?")]
    [Rule($"slot_declaration : {TypeIdentifierReference} (NamedKeyword {LongIdentifier})? ('at' {IdentifierReference})?")]
    private static SlotDeclaration MakeSlotDeclaration(
        TypeIdentifierReference identifierReferences,
        (IToken<EngLangTokenType>, IReadOnlyList<SymbolName> SlotName)? nameOverride,
        (IToken<EngLangTokenType>, IdentifierReference AliasFor)? alias)
    {
        var range = alias is null
            ? nameOverride is null
                ? identifierReferences.Range
                : new Yoakke.SynKit.Text.Range(identifierReferences.Range, nameOverride.Value.SlotName.Last().Range)
            : new Yoakke.SynKit.Text.Range(identifierReferences.Range, alias.Value.AliasFor.Range);
        string name;
        if (nameOverride != null)
        {
            name = string.Join(" ", nameOverride.Value.SlotName.Select(_ => _.Name));
        }
        else
        {
            name = identifierReferences.Name;
        }

        return new SlotDeclaration(
            name,
            identifierReferences.Name,
            identifierReferences.IsCollection,
            null,
            alias?.AliasFor?.Name.Name,
            range);
    }
    [Rule($"slot_declaration : IntLiteral {LongIdentifier} (NamedKeyword {LongIdentifier})? ('at' {IdentifierReference})?")]
    private static SlotDeclaration MakeSlotDeclaration(
        IToken<EngLangTokenType> sizeToken,
        IReadOnlyList<SymbolName> identifierReferences,
        (IToken<EngLangTokenType>, IReadOnlyList<SymbolName> SlotName)? nameOverride,
        (IToken<EngLangTokenType>, IdentifierReference AliasFor)? alias)
    {
        var range = alias is null
            ? nameOverride is null
                ? new Yoakke.SynKit.Text.Range(sizeToken.Range, identifierReferences.Last().Range)
                : new Yoakke.SynKit.Text.Range(sizeToken.Range, nameOverride.Value.SlotName.Last().Range)
            : new Yoakke.SynKit.Text.Range(sizeToken.Range, alias.Value.AliasFor.Range);
        string name;
        string typeName = string.Join(" ", identifierReferences.Select(_ => _.Name));
        if (nameOverride != null)
        {
            name = string.Join(" ", nameOverride.Value.SlotName.Select(_ => _.Name));
        }
        else
        {
            name = typeName;
        }

        return new SlotDeclaration(
            name,
            typeName,
            true,
            int.Parse(sizeToken.Text),
            alias?.AliasFor?.Name.Name,
            range);
    }

    [Rule($"comma_identifier_references_list : (slot_declaration (CommaKeyword slot_declaration)*)")]
    private static SlotDeclarationsList MakeCommaDelimitedIdentifierReferencesList(
        Punctuated<SlotDeclaration, IToken<EngLangTokenType>> identifierReferences)
        => new SlotDeclarationsList(identifierReferences.Values.ToImmutableList());

    [Rule($"label_word : {Identifier}")]
    [Rule($"label_word : OfKeyword")]
    [Rule($"label_word : AndKeyword")]
    [Rule($"label_word : AtKeyword")]
    [Rule($"label_word : OnKeyword")]
    [Rule($"label_word : ByKeyword")]
    [Rule($"label_word : MathOperationKeyword")]
    [Rule($"label_word : LogicalOperationKeyword")]
    [Rule($"label_word : EqualKeyword")]
    [Rule($"label_word : WithKeyword")]
    [Rule($"label_word : PutKeyword")]
    [Rule($"label_word : NamedKeyword")]
    //[Rule($"label_word : DefiniteArticleKeyword")]
    private static IToken<EngLangTokenType> MakeLabelWord(IToken<EngLangTokenType> marker)
        => marker;

    [Rule($"extended_label_word_strict : label_word")]
    [Rule($"extended_label_word_strict : IfKeyword")]
    [Rule($"extended_label_word_strict : IsKeyword")]
    [Rule($"extended_label_word_strict : HasKeyword")]
    [Rule($"extended_label_word_strict : AreKeyword")]
    //[Rule($"extended_label_word_strict : IntoKeyword")]
    // [Rule($"extended_label_word_strict : DefiniteArticleKeyword")]
    [Rule($"extended_label_word_strict : NullLiteral")]
    [Rule($"extended_label_word_strict : ToKeyword")]
    [Rule($"extended_label_word_strict : FromKeyword")]
    [Rule($"extended_label_word : FunctionBodyOrAsKeyword")]
    private static IToken<EngLangTokenType> MakeExtendedLabelWordStrict(IToken<EngLangTokenType> marker)
        => marker;

    [Rule($"extended_label_word : extended_label_word_strict")]
    [Rule($"extended_label_word : FunctionBodyOrAsKeyword")]
    private static IToken<EngLangTokenType> MakeExtendedLabelWord(IToken<EngLangTokenType> marker)
        => marker;

    [Rule($"extended_def_label_word : extended_label_word")]
    [Rule($"extended_def_label_word : DefiniteArticleKeyword")]
    private static IToken<EngLangTokenType> MakeDefinitionLabelWord(IToken<EngLangTokenType> marker)
        => marker;

    [Rule($"extended_def_label_word_strict : extended_label_word_strict")]
    [Rule($"extended_def_label_word_strict : DefiniteArticleKeyword")]
    private static IToken<EngLangTokenType> MakeDefinitionLabelWordStrict(IToken<EngLangTokenType> marker)
        => marker;

    [Rule($"comment_label : '(' ({Identifier} | '-' | '/' | IntLiteral | RatioLiteral | StringLiteral | WithKeyword | DefiniteArticleKeyword | IndefiniteArticleKeyword | IntoKeyword | FunctionBodyOrAsKeyword | MathOperationKeyword | ByKeyword | OfKeyword | HasKeyword | AndKeyword | IsKeyword | PutKeyword | TemperatureLiteral | EqualKeyword | NullLiteral | FromKeyword | ToKeyword | EllipsisKeyword | AtKeyword | LogicalOperationKeyword | AreKeyword)* ')'")]
    private static CommentLabel MakeCommentLabel(
        IToken<EngLangTokenType> toToken,
        IReadOnlyList<IToken<EngLangTokenType>> names,
        IToken<EngLangTokenType> colonToken)
    {
        string labelName = string.Join(" ", names.Select(token => token.Text));
        return new CommentLabel($"({labelName})", new(toToken.Range.Start, colonToken.Range.End));
    }

    [Rule($"invokable_label_saving_section : IntoKeyword extended_def_label_word_strict* parameter_references_list_strict")]
    private static (IToken<EngLangTokenType> intoToken, IEnumerable<IToken<EngLangTokenType>> OtherWords, (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) OutParameters) MakeInvokableLabelSavingSection(
        IToken<EngLangTokenType> intoToken, IEnumerable<IToken<EngLangTokenType>> OtherWords, (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) OutParameters)
    {
        return (intoToken, OtherWords, OutParameters);
    }

    [Rule($"invokable_label_definition_strict : label_word extended_def_label_word_strict* parameter_references_list_strict invokable_label_saving_section* comment_label?")]
    private static InvokableLabelDefinition MakeInvokableLabelStrict(
        IToken<EngLangTokenType> firstToken,
        IReadOnlyList<IToken<EngLangTokenType>> otherInitialTokens,
        (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) identifierTokens,
        IReadOnlyList<(IToken<EngLangTokenType> intoToken, IEnumerable<IToken<EngLangTokenType>> OtherWords, (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) OutParameters)> outParameter,
        CommentLabel? comment)
    {
        return MakeInvokableLabel(firstToken, otherInitialTokens, identifierTokens, outParameter, comment);
    }

    [Rule($"invokable_label_definition : label_word extended_def_label_word* parameter_references_list invokable_label_saving_section* comment_label?")]
    private static InvokableLabelDefinition MakeInvokableLabel(
        IToken<EngLangTokenType> firstToken,
        IReadOnlyList<IToken<EngLangTokenType>> otherInitialTokens,
        (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) identifierTokens,
        IReadOnlyList<(IToken<EngLangTokenType> intoToken, IEnumerable<IToken<EngLangTokenType>> OtherWords, (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) OutParameters)> outParameter,
        CommentLabel? comment)
    {
        string labelName = string.Join(" ",
            new[] { firstToken }
            .Union(otherInitialTokens)
            .Union(identifierTokens.InnerText)
            .Union(outParameter.Select(_ => _.intoToken))
            .Union(outParameter.SelectMany(_ => _.OtherWords))
            .Union(outParameter.SelectMany(_ => _.OutParameters.InnerText))
            .Select(i => i.Text));
        string labelWithComment = labelName + (comment is null ? "" : " " + comment.Text);
        var last = comment is not null
            ? comment.Range.End
            : outParameter.Count > 0
                ? (outParameter.Last().OutParameters.Parameters.Count > 0 ? outParameter.Last().OutParameters.Parameters.Last().Range.End : outParameter.Last().OutParameters.InnerText.Count > 0 ? outParameter.Last().OutParameters.InnerText.Last().Range.End : outParameter.Last().OtherWords.Last().Range.End)
                : identifierTokens.Parameters.Count > 0 ? identifierTokens.Parameters.Last().Range.End : identifierTokens.InnerText.Count > 0
                    ? identifierTokens.InnerText.Last().Range.End
                    : otherInitialTokens.Count > 0 ? otherInitialTokens.Last().Range.End : firstToken.Range.End;
        var range = new Yoakke.SynKit.Text.Range(firstToken.Range.Start, last);
        return new InvokableLabelDefinition(
            labelWithComment,
            identifierTokens.Parameters.Union(outParameter.SelectMany(_ => _.OutParameters.Parameters) ?? []).ToArray(),
            null,
            range);
    }

    [Rule($"invokable_label_definition : label_word extended_def_label_word* parameter_references_list IntoKeyword {LongIdentifier} comment_label?")]
    private static InvokableLabelDefinition MakeInvokableLabel(
        IToken<EngLangTokenType> firstToken,
        IReadOnlyList<IToken<EngLangTokenType>> otherInitialTokens,
        (ImmutableList<IToken<EngLangTokenType>> InnerText, ImmutableList<IdentifierReference> Parameters) identifierTokens,
        IToken<EngLangTokenType> intoToken,
        IReadOnlyList<SymbolName> outParameter,
        CommentLabel? comment)
    {
        string labelName = string.Join(" ", new[] { firstToken }.Union(otherInitialTokens).Union(identifierTokens.InnerText).Select(i => i.Text).Union(new[] { intoToken.Text }).Union(outParameter.Select(_ => _.Name)));
        string labelWithComment = labelName + (comment is null ? "" : " " + comment.Text);
        var last = comment is not null
            ? comment.Range.End
            : outParameter.Last().Range.End;
        var range = new Yoakke.SynKit.Text.Range(firstToken.Range.Start, last);
        return new InvokableLabelDefinition(labelWithComment, identifierTokens.Parameters.ToArray(), null, range);
    }

    [Rule($"prefixed_invokable_label : ToKeyword invokable_label_definition")]
    [Rule($"prefixed_invokable_label : 'define' invokable_label_definition_strict")]
    [Rule($"prefixed_invokable_label : 'Define' invokable_label_definition_strict")]
    private static InvokableLabelDefinition MakePrefixedInvokableLabel(
        IToken<EngLangTokenType> toToken,
    InvokableLabelDefinition label) => new InvokableLabelDefinition(label.Marker, label.Parameters, label.ResultIdentifier, new Yoakke.SynKit.Text.Range(toToken.Range.Start, label.Range.End));

    [Rule($"invokable_label_aliases : (prefixed_invokable_label (';' prefixed_invokable_label)*)")]
    private static InvokableLabel MakeInvokableLabelAliases(
        Punctuated<InvokableLabelDefinition, IToken<EngLangTokenType>> labels)
    {
        var primaryLabel = labels.Values.First();
        if (labels.Count == 1)
        {
            return new InvokableLabel([primaryLabel.Marker], primaryLabel.Parameters, primaryLabel.ResultIdentifier, primaryLabel.Range);
        }

        var range = new Yoakke.SynKit.Text.Range(primaryLabel.Range.Start, labels.Values.Last().Range.End);
        return new InvokableLabel(labels.Values.Select(_ => _.Marker).ToArray(), primaryLabel.Parameters, primaryLabel.ResultIdentifier, range);
    }


    [Rule($"invokable_label : invokable_label_aliases ':'")]
    [Rule($"invokable_label : invokable_label_aliases '->'")]
    [Rule($"invokable_label : invokable_label_aliases 'as'")]
    private static InvokableLabel MakeInvokableLabel(
        InvokableLabel label,
        IToken<EngLangTokenType> colonToken) => new InvokableLabel(label.Markers, label.Parameters, label.ResultIdentifier, new Yoakke.SynKit.Text.Range(label.Range.Start, colonToken.Range.End));

    [Rule($"invokable_label : invokable_label_definition '->'")]
    private static InvokableLabel MakeTrivialInvokableLabel(
        InvokableLabelDefinition label,
        IToken<EngLangTokenType> asToken) => new InvokableLabel([label.Marker], label.Parameters, label.ResultIdentifier, new Yoakke.SynKit.Text.Range(label.Range.Start, asToken.Range.End));

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
        return new LabeledStatement(invokableLabel, invokableLabel.Parameters, statement, new Yoakke.SynKit.Text.Range(invokableLabel.Range, statement.Range));
    }

    [Rule($"labeled_statement : invokable_label")]
    private static LabeledStatement MakeLabeledStatement(
        InvokableLabel invokableLabel)
    {
        return new LabeledStatement(invokableLabel, invokableLabel.Parameters, new BlockStatement(ImmutableList<Statement>.Empty, default), invokableLabel.Range);
    }

    [Rule($"invocation_statement_saving_section : IntoKeyword extended_label_word* identifier_references_list")]
    private static (IToken<EngLangTokenType> intoToken, IReadOnlyList<IToken<EngLangTokenType>> OtherWords, (IEnumerable<IToken<EngLangTokenType>> InnerText, ExpressionList Identifiers) OutParameters) MakeInvokableStatementSavingSection(
        IToken<EngLangTokenType> intoToken, IReadOnlyList<IToken<EngLangTokenType>> OtherWords, (IEnumerable<IToken<EngLangTokenType>> InnerText, ExpressionList Identifiers) OutParameters)
    {
        return (intoToken, OtherWords, OutParameters);
    }

    [Rule($"invocation_statement : label_word extended_label_word* identifier_references_list invocation_statement_saving_section* comment_label?")]
    private static Statement MakeInvocationStatement(
        IToken<EngLangTokenType> firstToken,
        IReadOnlyList<IToken<EngLangTokenType>> otherInitialTokens,
        (IEnumerable<IToken<EngLangTokenType>> InnerText, ExpressionList Identifiers) identifierTokens,
        IReadOnlyList<(IToken<EngLangTokenType> intoToken, IReadOnlyList<IToken<EngLangTokenType>> OtherWords, (IEnumerable<IToken<EngLangTokenType>> InnerText, ExpressionList Identifiers) OutParameters)> outParameter,
        CommentLabel? comment)
    {
        string labelName = string.Join(" ",
            new[] { firstToken }
            .Union(otherInitialTokens)
            .Union(identifierTokens.InnerText)
            .Union(outParameter.Select(_ => _.intoToken))
            .Union(outParameter.SelectMany(_ => _.OtherWords))
            .Union(outParameter.SelectMany(_ => _.OutParameters.InnerText))
            .Select(i => i.Text));
        var last = comment is not null
            ? comment.Range.End
            : outParameter.Count > 0
                ? (outParameter.Last().OutParameters.Identifiers.IdentifierReferences.Count > 0
                    ? outParameter.Last().OutParameters.Identifiers.IdentifierReferences.Last().Range.End
                    : outParameter.Last().OutParameters.InnerText.Count() > 0
                        ? outParameter.Last().OutParameters.InnerText.Last().Range.End : outParameter.Last().OtherWords.Last().Range.End)
                : identifierTokens.Identifiers.IdentifierReferences.Count > 0 ? identifierTokens.Identifiers.IdentifierReferences.Last().Range.End : identifierTokens.InnerText.Count() > 0
                    ? identifierTokens.InnerText.Last().Range.End
                    : otherInitialTokens.Count > 0 ? otherInitialTokens.Last().Range.End : firstToken.Range.End;
        var range = new Yoakke.SynKit.Text.Range(firstToken.Range.Start, last);
        return new InvocationStatement(
            labelName + (comment is null ? "" : " " + comment.Text),
            identifierTokens.Identifiers.IdentifierReferences.Union(outParameter.SelectMany(_ => _.OutParameters.Identifiers.IdentifierReferences)).ToArray(), null, range);
    }

    public static SyntaxNode Parse(string sourceCode)
    {
        if (!sourceCode.Contains('.') && !sourceCode.Contains(':'))
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
            case "some":
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
