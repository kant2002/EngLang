using Xunit;

namespace EngLang.Tests;

public class ExpressionTests
{
    [Fact]
    public void AddValueToVariable()
    {
        var sentence = "add 42 to a value";

        var parseResult = EngLangParser.Parse(sentence);

        var additionExpression = Assert.IsType<InPlaceAdditionExpression>(parseResult);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.Addend);
        Assert.Equal(42, addend.Value);
        Assert.Equal("value", additionExpression.TargetVariable.Name);
    }

    [Fact]
    public void MathAddition()
    {
        var sentence = "a value plus 42.";

        var parseResult = EngLangParser.Parse(sentence);

        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements));
        var additionExpression = Assert.IsType<MathExpression>(expression.Expression);
        Assert.Equal(MathOperator.Plus, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name);
    }

    [Fact]
    public void SubtractValueFromVariable()
    {
        var sentence = "subtract 42 from a value";

        var parseResult = EngLangParser.Parse(sentence);

        var subtractExpression = Assert.IsType<InPlaceSubtractExpression>(parseResult);
        var subtrahend = Assert.IsType<IntLiteralExpression>(subtractExpression.Subtrahend);
        Assert.Equal(42, subtrahend.Value);
        Assert.Equal("value", subtractExpression.TargetVariable.Name);
    }

    [Fact]
    public void MathSubtract()
    {
        var sentence = "a value minus 42.";

        var parseResult = EngLangParser.Parse(sentence);

        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements));
        var additionExpression = Assert.IsType<MathExpression>(expression.Expression);
        Assert.Equal(MathOperator.Minus, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name);
    }

    [Fact]
    public void MultiplyVariableByValue()
    {
        var sentence = "multiply a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var multiplyExpression = Assert.IsType<InPlaceMultiplyExpression>(parseResult);
        var multiplicative = Assert.IsType<IntLiteralExpression>(multiplyExpression.Factor);
        Assert.Equal(42, multiplicative.Value);
        Assert.Equal("value", multiplyExpression.TargetVariable.Name);
    }

    [Fact]
    public void MathMultiplication()
    {
        var sentence = "a value multiply by 42.";

        var parseResult = EngLangParser.Parse(sentence);

        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements));
        var additionExpression = Assert.IsType<MathExpression>(expression.Expression);
        Assert.Equal(MathOperator.Multiply, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name);
    }

    [Fact]
    public void DivideVariableByValue()
    {
        var sentence = "divide a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var divisionExpression = Assert.IsType<InPlaceDivisionExpression>(parseResult);
        var divisor = Assert.IsType<IntLiteralExpression>(divisionExpression.Denominator);
        Assert.Equal(42, divisor.Value);
        Assert.Equal("value", divisionExpression.TargetVariable.Name);
    }

    [Fact]
    public void MathDivision()
    {
        var sentence = "a value divide by 42.";

        var parseResult = EngLangParser.Parse(sentence);

        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements));
        var additionExpression = Assert.IsType<MathExpression>(expression.Expression);
        Assert.Equal(MathOperator.Divide, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name);
    }

    [Theory]
    [InlineData("a value is 42.", LogicalOperator.Equals)]
    [InlineData("a value is not 42.", LogicalOperator.NotEquals)]
    [InlineData("a value less than 42.", LogicalOperator.Less)]
    [InlineData("a value smaller than 42.", LogicalOperator.Less)]
    [InlineData("a value greater than 42.", LogicalOperator.Greater)]
    [InlineData("a value bigger than 42.", LogicalOperator.Greater)]
    public void LogicalBinaryOperationsOperation(string sentence, LogicalOperator logicalOperator)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements));
        var additionExpression = Assert.IsType<LogicalExpression>(expression.Expression);
        Assert.Equal(logicalOperator, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name);
    }
}
