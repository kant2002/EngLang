namespace EngLang.Tests.LanguageConversion;

using EngLang.LanguageConversion;
using Xunit;

public abstract class BaseLanguageConversionTests
{
    protected abstract ILanguageConverter CreateConverter();

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

    [Fact]
    public void ConvertResultStatement()
    {
        var sentence = "result is 42.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedResultStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedResultStatementCode();

    [Fact]
    public void ConvertLabeledStatement()
    {
        var sentence = "to do something: result is 1.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedLabeledStatementCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedLabeledStatementCode();

    [Fact]
    public void ConvertLabeledStatementWithParameters()
    {
        var sentence = "To calculate area from a width and a height: result is 1.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedLabeledStatementParametersCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedLabeledStatementParametersCode();

    [Fact]
    public void ConvertInvocationStatementWithParameters()
    {
        var sentence = "calculate factorial of a previous number into a previous factorial.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedInvocationStatementParametersCode();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedInvocationStatementParametersCode();

    [Fact]
    public void ConvertObjectPropertiesAccess()
    {
        var sentence = "multiply a width of a rectangle by a height of a rectangle.";
        var parseResult = EngLangParser.Parse(sentence);
        var converter = CreateConverter();

        var result = converter.Convert(parseResult);

        var expectedCode = GetExpectedObjectPropertiesAccess();
        Assert.Equal(expectedCode, result);
    }

    protected abstract string GetExpectedObjectPropertiesAccess();
}
