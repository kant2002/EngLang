using Xunit;

namespace EngLang.Tests;

public class ExpressionTests
{
    [Fact]
    public void AddValueToVariable()
    {
        var sentence = "add 42 to a value";

        var parseResult = EngLangParser.Parse(sentence);

        var additionExpression = Assert.IsType<AdditionExpression>(parseResult);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.Addend);
        Assert.Equal(42, addend.Value);
        Assert.Equal("value", additionExpression.TargetVariable.Name);
    }

    [Fact]
    public void SubtractValueFromVariable()
    {
        var sentence = "subtract 42 from a value";

        var parseResult = EngLangParser.Parse(sentence);

        var subtractExpression = Assert.IsType<SubtractExpression>(parseResult);
        var subtrahend = Assert.IsType<IntLiteralExpression>(subtractExpression.Subtrahend);
        Assert.Equal(42, subtrahend.Value);
        Assert.Equal("value", subtractExpression.TargetVariable.Name);
    }

    [Fact]
    public void MultiplyVariableByValue()
    {
        var sentence = "multiply a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var multiplyExpression = Assert.IsType<MultiplyExpression>(parseResult);
        var multiplicative = Assert.IsType<IntLiteralExpression>(multiplyExpression.Factor);
        Assert.Equal(42, multiplicative.Value);
        Assert.Equal("value", multiplyExpression.TargetVariable.Name);
    }

    [Fact]
    public void DivideVariableByValue()
    {
        var sentence = "divide a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var divisionExpression = Assert.IsType<DivisionExpression>(parseResult);
        var divisor = Assert.IsType<IntLiteralExpression>(divisionExpression.Denominator);
        Assert.Equal(42, divisor.Value);
        Assert.Equal("value", divisionExpression.TargetVariable.Name);
    }
}
