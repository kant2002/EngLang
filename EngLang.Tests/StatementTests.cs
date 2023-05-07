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
        var additionExpression = Assert.IsType<InPlaceAdditionExpression>(additionStatement.Expression);
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
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[3]).Expression);
    }

    [Fact]
    public void MultipleStatementsSeparateBySemicolon()
    {
        var sentence = "add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42. ";

        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[3]).Expression);
    }

    [Fact]
    public void ParagraphSeparateByDot()
    {
        var sentence = @"To test: add 42 to a value.
subtract 42 from a value.
multiply a value by 42.
divide a value by 42.

";

        var parseResult = EngLangParser.Parse(sentence);

        var wrappedStatement = Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements);
        var labeledStatement = Assert.IsType<LabeledStatement>(wrappedStatement);
        var blockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[3]).Expression);
    }

    [Fact]
    public void ParagraphCanEndWithFileEnd()
    {
        var sentence = @"To test: add 42 to a value.
subtract 42 from a value.
multiply a value by 42.
divide a value by 42.";

        var parseResult = EngLangParser.Parse(sentence);

        var wrappedStatement = Assert.Single(Assert.IsType<BlockStatement>(parseResult).Statements);
        var labeledStatement = Assert.IsType<LabeledStatement>(wrappedStatement);
        var blockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        Assert.Equal(4, blockStatement.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[3]).Expression);
    }

    [Fact]
    public void ParagraphsSeparatedByEmptyLine()
    {
        var sentence = @"To test: add 42 to a value.
subtract 42 from a value.
multiply a value by 42.

divide a value by 42.

";

        var parseResult = EngLangParser.Parse(sentence);

        var statements = Assert.IsType<BlockStatement>(parseResult).Statements;
        Assert.Equal(2, statements.Count);
        var wrappedStatement = statements[0];
        var labeledStatement = Assert.IsType<LabeledStatement>(wrappedStatement);
        var blockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        Assert.Equal(3, blockStatement.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);

        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(statements[1]).Expression);
    }

    [Fact(Skip = "Need to make explicit AST classes for paragraphs, paragraph lists and compound statements instead of relying BlockStatement.")]
    public void MultipleParagraphs()
    {
        var sentence = @"the width is a number.
the height is a number.

To calculate area from a width and a height ->
  result is a width multiplied by a height.

";

        var parseResult = EngLangParser.Parse(sentence);

        var statements = Assert.IsType<BlockStatement>(parseResult).Statements;
        Assert.Equal(3, statements.Count);
        var widthDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[0]);
        Assert.Equal("width", widthDeclarationStatement.Declaration.Name);
        Assert.Equal("number", widthDeclarationStatement.Declaration.TypeName.Name);

        var heightDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[1]);
        Assert.Equal("height", heightDeclarationStatement.Declaration.Name);
        Assert.Equal("number", heightDeclarationStatement.Declaration.TypeName.Name);

        var labeledStatement = Assert.IsType<LabeledStatement>(statements[2]);
        var blockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        Assert.Single(blockStatement.Statements);
        Assert.IsType<MathExpression>(Assert.IsType<ResultStatement>(blockStatement.Statements[0]).Value);
    }

    [Fact]
    public void LastLabeledSentenceMissed()
    {
        var sentence = @"the width is a number.
the height is a number.
To calculate area from a width and a height ->
  result is a width multiplied by a height.

";

        var parseResult = EngLangParser.Parse(sentence);

        var statements = Assert.IsType<BlockStatement>(parseResult).Statements;
        Assert.Equal(3, statements.Count);
        var widthDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[0]);
        Assert.Equal("width", widthDeclarationStatement.Declaration.Name);
        Assert.Equal("number", widthDeclarationStatement.Declaration.TypeName.Name);

        var heightDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[1]);
        Assert.Equal("height", heightDeclarationStatement.Declaration.Name);
        Assert.Equal("number", heightDeclarationStatement.Declaration.TypeName.Name);

        var labeledStatement = Assert.IsType<LabeledStatement>(statements[2]);
        var blockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        Assert.Single(blockStatement.Statements);
        Assert.IsType<MathExpression>(Assert.IsType<ResultStatement>(blockStatement.Statements[0]).Value);
    }

    [Theory]
    [InlineData("result is 1.")]
    [InlineData("return 1.")]
    public void ResultStatement(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var resultStatement = Assert.IsType<ResultStatement>(statement);
        Assert.Equal(1, Assert.IsType<IntLiteralExpression>(resultStatement.Value).Value);
    }

    [Theory]
    [InlineData("to do something: result is 1.")]
    [InlineData("to do something-> result is 1.")]
    [InlineData("To do something: result is 1.")]
    [InlineData("To do something -> result is 1.")]
    public void LabeledStatement(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var labeledStatement = Assert.IsType<LabeledStatement>(statement);
        Assert.Equal("do something", labeledStatement.Marker);
        var innerBlockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        var resultStatement = Assert.IsType<ResultStatement>(Assert.Single(innerBlockStatement.Statements));
        Assert.Equal(1, Assert.IsType<IntLiteralExpression>(resultStatement.Value).Value);
    }

    [Theory]
    [InlineData("to do something a parameter identi: result is 1.", "do something")]
    [InlineData("to do something a parameter identi-> result is 1.", "do something")]
    [InlineData("To calculate area from a width and a height: result is 1.", "calculate area from")]
    [InlineData("To calculate area from a width and a height-> result is 1.", "calculate area from")]
    [InlineData("calculate area from a width and a height-> result is 1.", "calculate area from")]
    [InlineData("define calculate area from a width and a height as result is 1.", "calculate area from")]
    [InlineData("define the factorial of a number as result is 1.", "factorial of")]
    public void LabeledWithParameterStatement(string sentence, string marker)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);
        var labeledStatement = Assert.IsType<LabeledStatement>(statement);
        Assert.Equal(marker, labeledStatement.Marker);
        var innerBlockStatement = Assert.IsType<BlockStatement>(labeledStatement.Statement);
        var resultStatement = Assert.IsType<ResultStatement>(Assert.Single(innerBlockStatement.Statements));
        Assert.Equal(1, Assert.IsType<IntLiteralExpression>(resultStatement.Value).Value);
    }

    [Theory]
    [InlineData("calculate factorial of a previous number into a previous factorial.", "calculate factorial of")]
    public void LabeledStatementInvocation(string sentence, string marker)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);

        var invocationStatement = Assert.IsType<InvocationStatement>(statement);
        Assert.Equal(marker, invocationStatement.Marker);

        var parameter = Assert.Single(invocationStatement.Parameters);
        Assert.Equal("previous number", parameter.Name);

        Assert.Equal("previous factorial", invocationStatement.ResultIdentifier.Name);
    }

    [Theory]
    [InlineData("if.")]
    [InlineData("if number.")]
    [InlineData("if number is 0 then result is 1.")]
    [InlineData("result is a number times factorial of a number minus one.")]
    public void InvalidStatement(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        var statement = Assert.Single(blockStatement.Statements);

        var invocationStatement = Assert.IsType<InvalidStatement>(statement);
    }
}
