using EngLang.LanguageConversion;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BaseShapeTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertShapeDeclarationStatement()
    {
        var sentence = "an apple is an fruit.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedShapeDeclarationStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedShapeDeclarationStatementCode();

    [Fact]
    public void ConvertShapeDeclarationWithSlotsStatement()
    {
        var sentence = """
            a rectangle is a shape with
                a width
                and a height.
            """;
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedShapeDeclarationStatementWithSlotsCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedShapeDeclarationStatementWithSlotsCode();

    [Fact]
    public void ConvertSimpleShapeDeclarationWithSlotsStatement()
    {
        var sentence = """
            a rectangle has
                a width
                and a height.
            """;
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedSimpleShapeDeclarationStatementWithSlotsCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedSimpleShapeDeclarationStatementWithSlotsCode();

    [Fact]
    public void EncodeLongIndentifier()
    {
        var sentence = """
            a rectangle has
                a width, a height and a fill colour.
            """;
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedEncodeLongIndentifierCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedEncodeLongIndentifierCode();
}
