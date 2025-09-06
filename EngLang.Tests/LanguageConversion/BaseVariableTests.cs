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

    [Fact(Skip = "Right now it's not clear to me should I have type string as reserved name, or not. For now it is, but previously it wasn't since it's nice for C# interop")]
    public void ConvertVariableWithStringType()
    {
        var sentence = "the greetings is an string equal to \"Hello\"";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedVariableWithStringType();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedVariableWithStringType();

    [Fact]
    public void ConvertVariableWithStringName()
    {
        var sentence = "the string is an charseq equal to \"Hello\"";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedVariableWithStringName();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedVariableWithStringName();

    [Fact]
    public void ConvertVariableWithIntLiteral()
    {
        var sentence = "the answer is an number equal to 42";
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
