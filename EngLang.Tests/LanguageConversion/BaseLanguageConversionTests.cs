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
