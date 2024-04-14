using Xunit;

namespace EngLang.Tests;

public class IfTests
{

    [Fact]
    public void IfStatement()
    {
        var sentence = "if a number is 0 then add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(ifStatement.Then).Expression);
    }

    [Theory]
    [InlineData("if a number is 0 then add 42 to a value.", LogicalOperator.Equals)]
    [InlineData("if a number is not 0 then add 42 to a value.", LogicalOperator.NotEquals)]
    [InlineData("if a number smaller than 0 then add 42 to a value.", LogicalOperator.Less)]
    [InlineData("if a number less than 0 then add 42 to a value.", LogicalOperator.Less)]
    [InlineData("if a number is less than 0 then add 42 to a value.", LogicalOperator.Less)]
    [InlineData("if a number at most 0 then add 42 to a value.", LogicalOperator.LessOrEquals)]
    [InlineData("if a number is at most 0 then add 42 to a value.", LogicalOperator.LessOrEquals)]
    [InlineData("if a number greater than 0 then add 42 to a value.", LogicalOperator.Greater)]
    [InlineData("if a number bigger than 0 then add 42 to a value.", LogicalOperator.Greater)]
    [InlineData("if a number at least 0 then add 42 to a value.", LogicalOperator.GreaterOrEquals)]
    [InlineData("if a number is at least 0 then add 42 to a value.", LogicalOperator.GreaterOrEquals)]
    public void IfSmallerStatement(string sentence, LogicalOperator expected)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        var logicalExpression = Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.Equal(expected, logicalExpression.Operator);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(ifStatement.Then).Expression);
    }

    [Fact]
    public void LogicalExpressionWithBothVars()
    {
        var sentence = "if a number is a dummy then add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(ifStatement.Then).Expression);
    }

    [Fact]
    public void IfStatementMultipleThen()
    {
        var sentence = "if a number smaller than 0 then add 42 to a value; exit.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        var thenBlock = Assert.IsType<BlockStatement>(ifStatement.Then);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(thenBlock.Statements[0]).Expression);
        Assert.IsType<InvocationStatement>(thenBlock.Statements[1]);
    }

    [Fact]
    public void IfStatementCanExpressInvocationStatement()
    {
        var sentence = "if a number is 0 then exit.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.Equal("exit", Assert.IsType<InvocationStatement>(ifStatement.Then).Marker);
    }

    [Theory]
    [InlineData("if a number is 0 , exit.")]
    [InlineData("if a number is 0, exit.")]
    [InlineData("if a number is 0,exit.")]
    [InlineData("If a number is 0,exit.")]
    [InlineData("If a number is null,exit.")]
    [InlineData("If a number is nil,exit.")]
    [InlineData("If the number is nil,exit.")]
    public void CommaInsteadOfThen(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
        IfStatement ifStatement = Assert.IsType<IfStatement>(statement);
        Assert.IsType<LogicalExpression>(ifStatement.Condition);
        Assert.Equal("exit", Assert.IsType<InvocationStatement>(ifStatement.Then).Marker);
    }
}
