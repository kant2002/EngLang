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

    [Fact]
    public void ShapeDeclarationWithSlots()
    {
        var sentence = """
a rectangle is a shape with a width and a height.
""";

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
}
