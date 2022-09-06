using Xunit;

namespace EngLang.Tests.Statements;

public class AssignmentStatementTests
{
    [Fact]
    public void AssignmentStatement()
    {
        var sentence = "put 40 into a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var assignmentStatement = Assert.IsType<ExpressionStatement>(statement);
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
    }

    [Fact]
    public void AlternateAssignmentStatement()
    {
        var sentence = "let a value is 20.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var assignmentStatement = Assert.IsType<ExpressionStatement>(statement);
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
        Assert.Equal(20, Assert.IsType<IntLiteralExpression>(assignmentExpression.Expression).Value);
    }

    [Fact(Skip = "English-script derivative")]
    public void LongerAlternateAssignmentStatement()
    {
        var sentence = "let the initial value of a number be x.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var assignmentStatement = Assert.IsType<ExpressionStatement>(statement);
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
        Assert.Equal(20, Assert.IsType<IntLiteralExpression>(assignmentExpression.Expression).Value);
    }

    [Fact]
    public void AlternateAssignmentStatementWithExpression()
    {
        var sentence = "let a value is 20 plus 10.";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var assignmentStatement = Assert.IsType<ExpressionStatement>(statement);
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
        var additionExpression = Assert.IsType<MathExpression>(assignmentExpression.Expression);
        Assert.Equal(20, Assert.IsType<IntLiteralExpression>(additionExpression.FirstOperand).Value);
        Assert.Equal(10, Assert.IsType<IntLiteralExpression>(additionExpression.SecondOperand).Value);
    }

    [Theory]
    [InlineData("let a value equal 20.")]
    [InlineData("let a value equals 20.")]
    public void AssignmentStatementVariations(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var assignmentStatement = Assert.IsType<ExpressionStatement>(statement);
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
        Assert.Equal(20, Assert.IsType<IntLiteralExpression>(assignmentExpression.Expression).Value);
    }
}
