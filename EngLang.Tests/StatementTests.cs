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
        var additionStatement = Assert.IsType<AdditionStatement>(Assert.Single(blockStatement.Statements));
        var additionExpression = additionStatement.Expression;
        var addend = Assert.IsType<IntLiteralExpression>(additionExpression.Addend);
        Assert.Equal(42, addend.Value);
        Assert.Equal("value", additionExpression.TargetVariable.Name);
    }

    [Fact]
    public void MultipleStatementsSeparateByDot()
    {
        var sentence = "add 42 to a value. subtract 42 from a value. multiply a value by 42. divide a value by 42";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<AdditionStatement>(blockStatement.Statements[0]);
        Assert.IsType<SubtractStatement>(blockStatement.Statements[1]);
        Assert.IsType<MultiplyStatement>(blockStatement.Statements[2]);
        Assert.IsType<DivisionStatement>(blockStatement.Statements[3]);
    }

    [Fact]
    public void MultipleStatementsSeparateBySemicolon()
    {
        var sentence = "add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42. ";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<AdditionStatement>(blockStatement.Statements[0]);
        Assert.IsType<SubtractStatement>(blockStatement.Statements[1]);
        Assert.IsType<MultiplyStatement>(blockStatement.Statements[2]);
        Assert.IsType<DivisionStatement>(blockStatement.Statements[3]);
    }
}
