using Xunit;

namespace EngLang.Tests;

public class JavaScriptConversionTests
{
    [Fact]
    public void ConvertVariable()
    {
        var sentence = "the name is an apple.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "let name;\r\n";
        Assert.Equal(expectedCode, result);
    }

    [Fact]
    public void ConvertVariableWithStringLiteral()
    {
        var sentence = "the greetings is an string equal to \"Hello\"";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "let greetings = \"Hello\"";
        Assert.Equal(expectedCode, result);
    }

    [Fact]
    public void ConvertVariableWithIntLiteral()
    {
        var sentence = "the answer to all things is an number equal to 42";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "let answer_to_all_things = 42";
        Assert.Equal(expectedCode, result);
    }

    [Fact]
    public void ConvertAddition()
    {
        var sentence = "add 42 to a value";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value += 42";
        Assert.Equal(expectedCode, result);
    }

    [Fact]
    public void ConvertSubstraction()
    {
        var sentence = "substract 42 from a value";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value -= 42";
        Assert.Equal(expectedCode, result);
    }

    [Fact]
    public void ConvertMultiplication()
    {
        var sentence = "multiply a value by 42";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value *= 42";
        Assert.Equal(expectedCode, result);
    }

    [Fact]
    public void ConvertDivision()
    {
        var sentence = "divide a value by 42";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = new JavaScriptConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value /= 42";
        Assert.Equal(expectedCode, result);
    }
}
