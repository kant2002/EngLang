using Xunit;
using Yoakke.SynKit.Text;

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
        Assert.Equal("value", additionExpression.TargetVariable.Name.Name);
        Assert.Equal(new Range(new Position(0, 12), new Position(0, 17)), additionExpression.TargetVariable.Name.Range);
    }

    [Fact]
    public void SubtractValueFromVariable()
    {
        var sentence = "subtract 42 from a value";

        var parseResult = EngLangParser.Parse(sentence);

        var subtractExpression = Assert.IsType<InPlaceSubtractExpression>(parseResult);
        var subtrahend = Assert.IsType<IntLiteralExpression>(subtractExpression.Subtrahend);
        Assert.Equal(42, subtrahend.Value);
        Assert.Equal("value", subtractExpression.TargetVariable.Name.Name);
    }

    [Fact]
    public void MultiplyVariableByValue()
    {
        var sentence = "multiply a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var multiplyExpression = Assert.IsType<InPlaceMultiplyExpression>(parseResult);
        var multiplicative = Assert.IsType<IntLiteralExpression>(multiplyExpression.Factor);
        Assert.Equal(42, multiplicative.Value);
        Assert.Equal("value", multiplyExpression.TargetVariable.Name.Name);
    }

    [Fact]
    public void DivideVariableByValue()
    {
        var sentence = "divide a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var divisionExpression = Assert.IsType<InPlaceDivisionExpression>(parseResult);
        var divisor = Assert.IsType<IntLiteralExpression>(divisionExpression.Denominator);
        Assert.Equal(42, divisor.Value);
        Assert.Equal("value", divisionExpression.TargetVariable.Name.Name);
    }

    [Fact]
    public void UseNestedPropertiesInExpression()
    {
        var sentence = "multiply a width of a rectangle by a height of a rectangle";

        var parseResult = EngLangParser.Parse(sentence);

        var divisionExpression = Assert.IsType<InPlaceMultiplyExpression>(parseResult);
        var factor = Assert.IsType<VariableExpression>(divisionExpression.Factor).Identifier;
        var target = divisionExpression.TargetVariable;
        Assert.Equal("width", target.Name.Name);
        Assert.NotNull(target.Owner);
        Assert.Equal("rectangle", target.Owner.Name.Name);
        Assert.Equal("height", factor.Name.Name);
        Assert.NotNull(factor.Owner);
        Assert.Equal("rectangle", factor.Owner.Name.Name);
    }

    [Theory]
    [InlineData("a value divide by 42.", MathOperator.Divide)]
    [InlineData("a value divided by 42.", MathOperator.Divide)]
    [InlineData("a value multiply by 42.", MathOperator.Multiply)]
    [InlineData("a value multiply 42.", MathOperator.Multiply)]
    [InlineData("a value multiplied by 42.", MathOperator.Multiply)]
    [InlineData("a value minus 42.", MathOperator.Minus)]
    [InlineData("a value plus 42.", MathOperator.Plus)]
    [InlineData("a value + 42.", MathOperator.Plus)]
    [InlineData("a value - 42.", MathOperator.Minus)]
    [InlineData("a value * 42.", MathOperator.Multiply)]
    [InlineData("a value / 42.", MathOperator.Divide)]
    public void MathExpressions(string sentence, MathOperator mathOperator)
    {

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(paragraph.Statements));
        var additionExpression = Assert.IsType<MathExpression>(expression.Expression);
        Assert.Equal(mathOperator, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name.Name);
    }

    [Theory]
    [InlineData("a value divide by a dummy.", MathOperator.Divide)]
    [InlineData("a value divided by a dummy.", MathOperator.Divide)]
    [InlineData("a value multiply by a dummy.", MathOperator.Multiply)]
    [InlineData("a value multiply a dummy.", MathOperator.Multiply)]
    [InlineData("a value times a dummy.", MathOperator.Multiply)]
    [InlineData("a value multiplied by a dummy.", MathOperator.Multiply)]
    [InlineData("a value minus a dummy.", MathOperator.Minus)]
    [InlineData("a value plus a dummy.", MathOperator.Plus)]
    [InlineData("a value + a dummy.", MathOperator.Plus)]
    [InlineData("a value - a dummy.", MathOperator.Minus)]
    [InlineData("a value * a dummy.", MathOperator.Multiply)]
    [InlineData("a value / a dummy.", MathOperator.Divide)]
    public void MathExpressionsWithVar(string sentence, MathOperator mathOperator)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var expression = Assert.IsType<ExpressionStatement>(Assert.Single(paragraph.Statements));
        var additionExpression = Assert.IsType<MathExpression>(expression.Expression);
        Assert.Equal(mathOperator, additionExpression.Operator);
        var addend = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.SecondOperand).Identifier);
        Assert.Equal("dummy", addend.Name.Name);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name.Name);
    }

    [Theory]
    [InlineData("a value is 42", LogicalOperator.Equals)]
    [InlineData("a value is not 42", LogicalOperator.NotEquals)]
    [InlineData("a value less than 42", LogicalOperator.Less)]
    [InlineData("a value smaller than 42", LogicalOperator.Less)]
    [InlineData("a value greater than 42", LogicalOperator.Greater)]
    [InlineData("a value bigger than 42", LogicalOperator.Greater)]
    public void LogicalBinaryOperationsOperation(string sentence, LogicalOperator logicalOperator)
    {
        var lexer = new EngLangLexer(sentence);
        var parser = new EngLangParser(lexer);
        var parseResult = parser.ParseLogicalExpression();
        Assert.True(parseResult.IsOk);

        var additionExpression = (LogicalExpression)parseResult.Ok.Value;
        Assert.Equal(logicalOperator, additionExpression.Operator);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand);
        Assert.Equal(42, addend.Value);
        var variable = Assert.IsType<IdentifierReference>(Assert.IsType<VariableExpression>(additionExpression.FirstOperand).Identifier);
        Assert.Equal("value", variable.Name.Name);
    }

    [Fact]
    public void AddMathExpressionToVariable()
    {
        var sentence = "add 42 * 13 to a value";

        var parseResult = EngLangParser.Parse(sentence);

        var additionExpression = Assert.IsType<InPlaceAdditionExpression>(parseResult);
        var addend = Assert.IsType<MathExpression>(additionExpression.Addend);
        Assert.Equal(MathOperator.Multiply, addend.Operator);
        Assert.Equal(42, Assert.IsType<IntLiteralExpression>(addend.FirstOperand).Value);
        Assert.Equal(13, Assert.IsType<IntLiteralExpression>(addend.SecondOperand).Value);
        Assert.Equal("value", additionExpression.TargetVariable.Name.Name);
    }

    [Theory]
    [InlineData("\"Some string\"", "Some string")]
    [InlineData("\"\\\"", "\\")]
    [InlineData("\"\"\"\"", "\"")]
    public void StringLiteral(string sentence, string expected)
    {
        var parser = new EngLangParser(new EngLangLexer(sentence));
        var parseResult = parser.ParseExpression();

        Assert.True(parseResult.IsOk);
        var stringLiteralExpression = Assert.IsType<StringLiteralExpression>(parseResult.Ok.Value);
        Assert.Equal(expected, stringLiteralExpression.Value);
    }

    [Fact]
    public void IntLiteral()
    {
        var sentence = "42";

        var parser = new EngLangParser(new EngLangLexer(sentence));
        var parseResult = parser.ParseExpression();

        Assert.True(parseResult.IsOk);
        var intLiteralExpression = Assert.IsType<IntLiteralExpression>(parseResult.Ok.Value);
        Assert.Equal(42, intLiteralExpression.Value);
    }

    [Fact]
    public void ByteArrayLiteral()
    {
        var sentence = "0x0102030405060708";

        var parser = new EngLangParser(new EngLangLexer(sentence));
        var parseResult = parser.ParseExpression();

        Assert.True(parseResult.IsOk);
        var intLiteralExpression = Assert.IsType<ByteArrayLiteralExpression>(parseResult.Ok.Value);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, intLiteralExpression.Value);
    }

    [Fact]
    public void InchLiteral()
    {
        var sentence = "42 inch";

        var parser = new EngLangParser(new EngLangLexer(sentence));
        var parseResult = parser.ParseExpression();

        Assert.True(parseResult.IsOk);
        var intLiteralExpression = Assert.IsType<InchLiteralExpression>(parseResult.Ok.Value);
        Assert.Equal(42, intLiteralExpression.Value);
    }

    [Fact]
    public void RatioLiteral()
    {
        var sentence = "42/22";

        var parser = new EngLangParser(new EngLangLexer(sentence));
        var parseResult = parser.ParseExpression();

        Assert.True(parseResult.IsOk);
        var intLiteralExpression = Assert.IsType<RatioLiteralExpression>(parseResult.Ok.Value);
        Assert.Equal(42, intLiteralExpression.Numerator);
        Assert.Equal(22, intLiteralExpression.Denominator);
    }

    [Theory]
    [InlineData("a terminal's color")]
    [InlineData("a color of a terminal")]
    public void PosessiveForm(string sentence)
    {
        var lexer = new EngLangLexer(sentence);
        var parser = new EngLangParser(lexer);
        var parseResult = parser.ParseExpression();
        Assert.True(parseResult.IsOk);

        var expression = parseResult.Ok.Value;
        var variableExpression = Assert.IsType<VariableExpression>(expression);
        Assert.Equal("color", variableExpression.Identifier.Name.Name);
        Assert.Equal("terminal", variableExpression.Identifier.Owner.Name.Name);
    }

    [Fact]
    public void StringPosessiveForm()
    {
        var lexer = new EngLangLexer("\" blabla \"'s pointer");
        var parser = new EngLangParser(lexer);
        var parseResult = parser.ParseExpression();
        Assert.True(parseResult.IsOk);

        var expression = parseResult.Ok.Value;
        var variableExpression = Assert.IsType<PosessiveExpression>(expression);
        Assert.Equal("pointer", variableExpression.Identifier.Name.Name);
        Assert.Equal(new Range(new Position(0, 13), new Position(0, 20)), variableExpression.Identifier.Name.Range);
        Assert.IsType<StringLiteralExpression>(variableExpression.Owner);
    }
}
