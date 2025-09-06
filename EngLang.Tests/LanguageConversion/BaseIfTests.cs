using EngLang.LanguageConversion;
using Xunit;

namespace EngLang.Tests.LanguageConversion;

public abstract class BaseIfTests
{
    protected abstract ILanguageConverter CreateConverter();

    [Fact]
    public void ConvertIfEqualsStatement()
    {
        var sentence = "if a number is 0 then add 42 to a value.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedIfEqualsStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedIfEqualsStatementCode();

    [Fact]
    public void ConvertIfLessThanStatement()
    {
        var sentence = "if a number smaller than 0 then add 42 to a value.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedIfLessThanStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedIfLessThanStatementCode();

    [Fact]
    public void ConvertIfMultipleThenStatement()
    {
        var sentence = "if a number is 0 then add 42 to a value; exit.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedIfMultipleThenStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedIfMultipleThenStatementCode();

    [Fact]
    public void ConvertResultInsideIfStatement()
    {
        var sentence = "if a number smaller than 0 then result is 42.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedResultInsideIfStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedResultInsideIfStatementCode();

    [Fact]
    public void ConvertIfWithFlagSetStatement()
    {
        var sentence = "if a flag is set then result is 42.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedIfWithFlagSetStatement();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedIfWithFlagSetStatement();
}
