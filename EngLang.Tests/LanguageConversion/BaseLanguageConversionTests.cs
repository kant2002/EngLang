namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using Xunit;

public abstract class BaseLanguageConversionTests
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
    public void ConvertAddition()
    {
        var sentence = "add 42 to a value";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value += 42";
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedAdditionCode();

    [Fact]
    public void ConvertSubtraction()
    {
        var sentence = "subtract 42 from a value";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value -= 42";
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedSubtractionCode();

    [Fact]
    public void ConvertMultiplication()
    {
        var sentence = "multiply a value by 42";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = "value *= 42";
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedMultiplicationCode();

    [Fact]
    public void ConvertDivision()
    {
        var sentence = "divide a value by 42";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedDivisionCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedDivisionCode();

    [Fact]
    public void ConvertAssignment()
    {
        var sentence = "put 40 into a value";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedAssignmentCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedAssignmentCode();

    [Fact]
    public void ConvertStatements()
    {
        var sentence = "put 40 into a value. add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedStatementsCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedStatementsCode();
}
