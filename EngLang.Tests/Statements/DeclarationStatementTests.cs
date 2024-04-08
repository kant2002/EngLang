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
        Assert.Equal("apple", shapeDeclaration.Name);
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
        Assert.Equal("rectangle", shapeDeclaration.Name);
        Assert.Equal("shape", shapeDeclaration.BaseShapeName.Name);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Value[0].Name);
        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Value[1].Name);
    }

    [Theory]
    [InlineData("""
An rectangle has a width and a height.
""")]
    [InlineData("""
An rectangle has a width, a height.
""")]
    public void ShapeDeclarationWithSlotsWithoutBase(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        var shapeStatement = Assert.IsType<ShapeDeclarationStatement>(statement);
        var shapeDeclaration = shapeStatement.Declaration;
        Assert.Equal("rectangle", shapeDeclaration.Name);
        Assert.Null(shapeDeclaration.BaseShapeName);
        Assert.NotNull(shapeDeclaration.WellKnownSlots);
        Assert.Equal("width", shapeDeclaration.WellKnownSlots.Value[0].Name);
        Assert.Equal("height", shapeDeclaration.WellKnownSlots.Value[1].Name);
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
        Assert.Equal("rectangle", variableExpression.Identifier.Name);
        Assert.NotNull(variableExpression.Identifier.Owner);
        Assert.Equal("sprite", variableExpression.Identifier.Owner.Name);
    }
}
