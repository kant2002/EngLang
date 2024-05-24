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
}
