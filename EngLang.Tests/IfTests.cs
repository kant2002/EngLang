using Xunit;

namespace EngLang.Tests;

public class IfTests
{

    [Fact]
    public void IfStatement()
    {
        var sentence = "if a number is 0 then add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(ifStatement.Then).Expression);
    }

    [Fact]
    public void IfSmallerStatement()
    {
        var sentence = "if a number smaller than 0 then add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(ifStatement.Then).Expression);
    }

    [Fact]
    public void IfStatementCanExpressInvocationStatement()
    {
        var sentence = "if a number is 0 then exit.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.Equal("exit", Assert.IsType<InvocationStatement>(ifStatement.Then).Marker);
    }

    [Theory]
    [InlineData("if a number is 0 , exit.")]
    [InlineData("if a number is 0, exit.")]
    [InlineData("if a number is 0,exit.")]
    public void CommaInsteadOfThen(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.Equal("exit", Assert.IsType<InvocationStatement>(ifStatement.Then).Marker);
    }
}
