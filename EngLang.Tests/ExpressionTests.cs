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
    public void SubstractValueFromVariable()
    {
        var sentence = "substract 42 from a value";

        var parseResult = EngLangParser.Parse(sentence);

        var substractExpression = Assert.IsType<SubtractExpression>(parseResult);
        var subtrahend = Assert.IsType<IntLiteralExpression>(substractExpression.Subtrahend);
        Assert.Equal(42, subtrahend.Value);
        Assert.Equal("value", substractExpression.TargetVariable.Name);
    }

    [Fact]
    public void MultiplyVariableByValue()
    {
        var sentence = "multiply a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var substractExpression = Assert.IsType<MultiplyExpression>(parseResult);
        var subtrahend = Assert.IsType<IntLiteralExpression>(substractExpression.Factor);
        Assert.Equal(42, subtrahend.Value);
        Assert.Equal("value", substractExpression.TargetVariable.Name);
    }

    [Fact]
    public void DivideVariableByValue()
    {
        var sentence = "divide a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var substractExpression = Assert.IsType<DivisionExpression>(parseResult);
        var subtrahend = Assert.IsType<IntLiteralExpression>(substractExpression.Denominator);
        Assert.Equal(42, subtrahend.Value);
        Assert.Equal("value", substractExpression.TargetVariable.Name);
    }
}
