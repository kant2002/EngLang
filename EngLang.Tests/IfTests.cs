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

    [Fact]
    public void IfSmallerStatement()
    {
        var sentence = "if a number smaller than 0 then add 42 to a value.";

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
