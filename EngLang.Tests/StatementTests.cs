using Xunit;

namespace EngLang.Tests;

public class StatementTests
{
    [Fact]
    public void AddValueToVariableStatement()
    {
        var sentence = "add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var additionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(blockStatement.Statements));
        var additionExpression = Assert.IsType<AdditionExpression>(additionStatement.Expression);
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.Addend);
        Assert.Equal(42, addend.Value);
        Assert.Equal("value", additionExpression.TargetVariable.Name);
    }

    [Fact]
    public void MultipleStatementsSeparateByDot()
    {
        var sentence = "add 42 to a value. subtract 42 from a value. multiply a value by 42. divide a value by 42.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<AdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<SubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<MultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);
        Assert.IsType<DivisionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[3]).Expression);
    }

    [Fact]
    public void MultipleStatementsSeparateBySemicolon()
    {
        var sentence = "add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42. ";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<AdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<SubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<MultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);
        Assert.IsType<DivisionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[3]).Expression);
    }

    [Fact]
    public void IfStatement()
    {
        var sentence = "if a number is 0 then add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<EqualityExpression>(ifStatement.Condition);
        Assert.IsType<AdditionExpression>(Assert.IsType<ExpressionStatement>(ifStatement.Then).Expression);
    }
}
