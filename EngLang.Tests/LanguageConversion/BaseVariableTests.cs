using EngLang.LanguageConversion;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BaseVariableTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertVariable()
    {
        var sentence = "the name is an apple.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedVariable();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedVariable();

    [Fact]
    public void ConvertVariableWithStringLiteral()
    {
        var sentence = "the greetings is an string equal to \"Hello\"";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedVariableWithStringLiteral();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedVariableWithStringLiteral();

    [Fact]
    public void ConvertVariableWithIntLiteral()
    {
        var sentence = "the answer to all things is an number equal to 42";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedVariableWithIntLiteral();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedVariableWithIntLiteral();

    [Fact]
    public void ConvertVariableWithDash()
    {
        var sentence = "a zero-index variable";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedVariableWithDash();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedVariableWithDash();
}
