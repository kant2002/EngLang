using Xunit;

namespace EngLang.Tests.Statements;

public class DeclarationStatementTests
{
    [Fact]
    public void ShapeDeclaration()
    {
        var sentence = "an apple is an fruit.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("apple", shapeDeclaration.Name.Name);
        Assert.Equal("fruit", shapeDeclaration.BaseShapeName.Name);
        Assert.Null(shapeDeclaration.WellKnownSlots);
    }

    [Theory]
    [InlineData("""
a rectangle is a shape with a width and a height.
""")]
    //[InlineData("A rectangle is a shape with a width and a height.")]
    [InlineData("""
an rectangle is a shape with a width and a height.
""")]
    [InlineData("""
An rectangle is a shape with a width and a height.
""")]
    [InlineData("""
the rectangle is a shape with a width and a height.
""")]
    [InlineData("""
an rectangle is a shape with a width, a height.
""")]
    [InlineData("""
an rectangle is a shape with a width, Some height.
""")]
    public void ShapeDeclarationWithSlots(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("rectangle", shapeDeclaration.Name.Name);
        Assert.Equal("shape", shapeDeclaration.BaseShapeName.Name);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Slots[1].Name);
    }

    [Theory]
    [InlineData("""
An rectangle has a width and a height, a fill colour.
""")]
    [InlineData("""
An rectangle has a width, a height and a fill colour.
""")]
    [InlineData("""
An rectangle has a width, a height, and a fill colour.
""")]
    [InlineData("""
An rectangle has some widths, a height, and some fill colour.
""")]
    [InlineData("""
An rectangle has some widths, or a height and some fill colour.
""")]
    public void ShapeDeclarationWithSlotsWithoutBase(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("rectangle", shapeDeclaration.Name.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].AliasFor);
        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[1].AliasFor);
        Assert.Equal("fill colour", shapeDeclaration.WellKnownSlots.Slots[2].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[2].AliasFor);
    }

    [Theory]
    [InlineData("""
The rectangle has a width, a height, a fill colour.
""")]
    public void GlobalShapeDeclarationWithSlotsWithoutBase(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("rectangle", shapeDeclaration.Name.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].AliasFor);
        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[1].AliasFor);
        Assert.Equal("fill colour", shapeDeclaration.WellKnownSlots.Slots[2].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[2].AliasFor);
    }

    [Theory]
    [InlineData("""
a pen has
    a width,
    a size is a width.
""")]
    [InlineData("""
a pen has
    a width and
    a size is a width.
""")]
    [InlineData("""
a pen has
    a width,
    a size at the width.
""")]
    [InlineData("""
a pen has
    a width and
    a size at the width.
""")]
    [InlineData("""
a pen has
    a width or
    a size at the width.
""")]
    public void ShapeDeclarationWithSlotsAndAlias(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("pen", shapeDeclaration.Name.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].AliasFor);
        Assert.Equal("size", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[1].AliasFor);
    }

    [Theory]
    [InlineData("""
a pen has
    a width,
    a number named size.
""")]
    [InlineData("""
the pen has
    a width,
    a number named size.
""")]
    public void ShapeDeclarationWithSlotsAndNames(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("pen", shapeDeclaration.Name.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].TypeName);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].AliasFor);

        Assert.Equal("size", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Equal("number", shapeDeclaration.WellKnownSlots.Slots[1].TypeName);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[1].CollectionSize);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[1].AliasFor);
    }

    [Theory]
    [InlineData("""
a pen has
    a width,
    16 bytes named name.
""")]
    public void ShapeDeclarationWithByteArray(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("pen", shapeDeclaration.Name.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].TypeName);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].AliasFor);

        Assert.Equal("name", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Equal("bytes", shapeDeclaration.WellKnownSlots.Slots[1].TypeName);
        Assert.Equal(16, shapeDeclaration.WellKnownSlots.Slots[1].CollectionSize);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[1].AliasFor);
    }

    [Theory]
    [InlineData("""
a pen has
    a width,
    a height (the comment).
""")]
    public void ShapeDeclarationWithComment(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("pen", shapeDeclaration.Name.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Slots[0].TypeName);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].Comment);

        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Slots[1].TypeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots.Slots[1].Comment);
        Assert.Equal("(the comment)", shapeDeclaration.WellKnownSlots.Slots[1].Comment.Text);
    }

    [Theory]
    [InlineData("a rectangle of a sprite.")]
    [InlineData("a sprite's rectangle.")]
    public void PosessionDeclaration(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var variableExpression = Assert.IsType<VariableExpression>(expressionStatement.Expression);
        Assert.Equal("rectangle", variableExpression.Identifier.Name.Name);
        Assert.NotNull(variableExpression.Identifier.Owner);
        Assert.Equal("sprite", variableExpression.Identifier.Owner.Name.Name);
    }

    [Theory]
    [InlineData("a sprite's rectangle's width.")]
    //[InlineData("a width of sprite's rectangle.")]
    public void PosessionDeclarationNesting(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var variableExpression = Assert.IsType<VariableExpression>(expressionStatement.Expression);
        Assert.Equal("width", variableExpression.Identifier.Name.Name);
        Assert.NotNull(variableExpression.Identifier.Owner);
        Assert.Equal("rectangle", variableExpression.Identifier.Owner.Name.Name);
        Assert.NotNull(variableExpression.Identifier.Owner.Owner);
        Assert.Equal("sprite", variableExpression.Identifier.Owner.Owner.Name.Name);
    }

    [Theory]
    [InlineData("some method procedures is a record with a getname pointer and a tostring pointer.")]
    public void ShapeForMultipleSameItems(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("method procedures", shapeDeclaration.Name.Name);
        Assert.NotNull(shapeDeclaration.BaseShapeName);
        Assert.Equal("record", shapeDeclaration.BaseShapeName.Name);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("getname pointer", shapeDeclaration.WellKnownSlots.Slots[0].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[0].AliasFor);
        Assert.Equal("tostring pointer", shapeDeclaration.WellKnownSlots.Slots[1].Name);
        Assert.Null(shapeDeclaration.WellKnownSlots.Slots[1].AliasFor);
    }

    [Theory]
    [InlineData("A data pointer is a pointer to a data.")]
    public void PointerTypeDeclaration(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var pointerDeclarationStatement = Assert.IsType<PointerDeclarationStatement>(statement);
        Assert.Equal("data pointer", pointerDeclarationStatement.PointerType.Name.Name);
        Assert.NotNull(pointerDeclarationStatement.BaseType);
        Assert.Equal("data", pointerDeclarationStatement.BaseType.Name);
    }

    [Fact]
    public void TypedVariableWithIntConstantDeclaration()
    {
        var sentence = "The apple is an fruit equal to 5.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var variableStatement = Assert.IsType<VariableDeclarationStatement>(statement);
        var variableDeclaration = variableStatement.Declaration;
        Assert.Equal("apple", variableDeclaration.Name);
        Assert.Equal("fruit", variableDeclaration.TypeName.Name);
        Assert.NotNull(variableDeclaration.Expression);
        var literal = Assert.IsType<IntLiteralExpression>(variableDeclaration.Expression);
        Assert.Equal(5, literal.Value);
    }

    [Fact]
    public void TypedVariableWithRatioConstantDeclaration()
    {
        var sentence = "The apple is an fruit equal to 5/2.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var variableStatement = Assert.IsType<VariableDeclarationStatement>(statement);
        var variableDeclaration = variableStatement.Declaration;
        Assert.Equal("apple", variableDeclaration.Name);
        Assert.Equal("fruit", variableDeclaration.TypeName.Name);
        Assert.NotNull(variableDeclaration.Expression);
        var literal = Assert.IsType<RatioLiteralExpression>(variableDeclaration.Expression);
        Assert.Equal(5, literal.Numerator);
        Assert.Equal(2, literal.Denominator);
    }

    [Fact]
    public void TypedVariableWithInchConstantDeclaration()
    {
        var sentence = "The apple is an fruit equal to 5/2 inch.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var variableStatement = Assert.IsType<VariableDeclarationStatement>(statement);
        var variableDeclaration = variableStatement.Declaration;
        Assert.Equal("apple", variableDeclaration.Name);
        Assert.Equal("fruit", variableDeclaration.TypeName.Name);
        Assert.NotNull(variableDeclaration.Expression);
        var inchLiteral = Assert.IsType<InchLiteralExpression>(variableDeclaration.Expression);
        var literal = Assert.IsType<RatioLiteralExpression>(inchLiteral.Value);
        Assert.Equal(5, literal.Numerator);
        Assert.Equal(2, literal.Denominator);
    }

    [Fact]
    public void TypedVariableWithInchConstantDeclaration2()
    {
        var sentence = "The apple is an fruit equal to 2 inches.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var variableStatement = Assert.IsType<VariableDeclarationStatement>(statement);
        var variableDeclaration = variableStatement.Declaration;
        Assert.Equal("apple", variableDeclaration.Name);
        Assert.Equal("fruit", variableDeclaration.TypeName.Name);
        Assert.NotNull(variableDeclaration.Expression);
        var inchLiteral = Assert.IsType<InchLiteralExpression>(variableDeclaration.Expression);
        var literal = Assert.IsType<IntLiteralExpression>(inchLiteral.Value);
        Assert.Equal(2, literal.Value);
    }
}
