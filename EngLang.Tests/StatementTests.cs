using System.Linq;
using Xunit;

namespace EngLang.Tests;

public class StatementTests
{
    [Fact]
    public void AddValueToVariableStatement()
    {
        var sentence = "add 42 to a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var additionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(paragraph.Statements));
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

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.Equal(4, paragraph.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[3]).Expression);
    }

    [Fact]
    public void MultipleStatementsSeparateBySemicolon()
    {
        var sentence = "add 42 to a value; subtract 42 from a value; multiply a value by 42; divide a value by 42. ";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.Equal(4, paragraph.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[3]).Expression);
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

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var blockStatement = paragraph;
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

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.Equal(4, paragraph.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[2]).Expression);
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(paragraph.Statements[3]).Expression);
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

        var paragraphs = Assert.IsType<ParagraphList>(parseResult).Paragraphs;
        Assert.Equal(2, paragraphs.Count);
        var paragraph = paragraphs[0];
        var blockStatement = paragraph;
        Assert.Equal(3, blockStatement.Statements.Count);
        Assert.IsType<InPlaceAdditionExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[0]).Expression);
        Assert.IsType<InPlaceSubtractExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[1]).Expression);
        Assert.IsType<InPlaceMultiplyExpression>(Assert.IsType<ExpressionStatement>(blockStatement.Statements[2]).Expression);

        var paragraph2 = paragraphs[1];
        Assert.IsType<InPlaceDivisionExpression>(Assert.IsType<ExpressionStatement>(Assert.Single(paragraph2.Statements)).Expression);
    }

    [Fact]
    public void MultipleParagraphs()
    {
        var sentence = @"the width is a number.
the height is a number.

To calculate area from a width and a height ->
  result is a width multiplied by a height.

";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraphs = Assert.IsType<ParagraphList>(parseResult).Paragraphs;
        Assert.Equal(2, paragraphs.Count);

        var statements = paragraphs[0].Statements;
        Assert.Equal(2, statements.Count);
        var widthDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[0]);
        Assert.Equal("width", widthDeclarationStatement.Declaration.Name);
        Assert.Equal("number", widthDeclarationStatement.Declaration.TypeName.Name);

        var heightDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[1]);
        Assert.Equal("height", heightDeclarationStatement.Declaration.Name);
        Assert.Equal("number", heightDeclarationStatement.Declaration.TypeName.Name);

        var blockStatement = paragraphs[1];
        Assert.Single(blockStatement.Statements);
        Assert.IsType<MathExpression>(Assert.IsType<ResultStatement>(blockStatement.Statements[0]).Value);
    }

    [Fact]
    public void MultipleParagraphsStartingFromEmptyLine()
    {
        var sentence = @"

the width is a number.
the height is a number.

To calculate area from a width and a height ->
  result is a width multiplied by a height.

";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraphs = Assert.IsType<ParagraphList>(parseResult).Paragraphs;
        Assert.Equal(2, paragraphs.Count);

        var statements = paragraphs[0].Statements;
        Assert.Equal(2, statements.Count);
        var widthDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[0]);
        Assert.Equal("width", widthDeclarationStatement.Declaration.Name);
        Assert.Equal("number", widthDeclarationStatement.Declaration.TypeName.Name);

        var heightDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[1]);
        Assert.Equal("height", heightDeclarationStatement.Declaration.Name);
        Assert.Equal("number", heightDeclarationStatement.Declaration.TypeName.Name);

        var blockStatement = paragraphs[1];
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

        var paragraphs = Assert.IsType<ParagraphList>(parseResult).Paragraphs;
        Assert.Equal(2, paragraphs.Count);
        var paragraph = paragraphs[0];
        var statements = paragraph.Statements;
        Assert.Equal(2, statements.Count);
        var widthDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[0]);
        Assert.Equal("width", widthDeclarationStatement.Declaration.Name);
        Assert.Equal("number", widthDeclarationStatement.Declaration.TypeName.Name);

        var heightDeclarationStatement = Assert.IsType<VariableDeclarationStatement>(statements[1]);
        Assert.Equal("height", heightDeclarationStatement.Declaration.Name);
        Assert.Equal("number", heightDeclarationStatement.Declaration.TypeName.Name);

        var labeledParagraph = paragraphs[1];
        var labeledParagraphStatements = Assert.Single(labeledParagraph.Statements);
        Assert.IsType<MathExpression>(Assert.IsType<ResultStatement>(labeledParagraphStatements).Value);
    }

    [Theory]
    [InlineData("result is 1.")]
    [InlineData("return 1.")]
    public void ResultStatement(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);
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

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.NotNull(paragraph.Label);
        var labeledStatement = paragraph.Label;
        Assert.Equal("do something", labeledStatement.Markers.First());
        var statement = Assert.Single(paragraph.Statements);
        var resultStatement = Assert.IsType<ResultStatement>(statement);
        Assert.Equal(1, Assert.IsType<IntLiteralExpression>(resultStatement.Value).Value);
    }

    [Theory]
    [InlineData("to do something a parameter identi: result is 1.", "do something")]
    [InlineData("to do something a parameter identi-> result is 1.", "do something")]
    [InlineData("To calculate area from a width and a height: result is 1.", "calculate area from and")]
    [InlineData("To calculate area from a width and a height-> result is 1.", "calculate area from and")]
    [InlineData("calculate area from a width and a height-> result is 1.", "calculate area from and")]
    [InlineData("define calculate area from a width and a height as result is 1.", "calculate area from and")]
    [InlineData("Define calculate area from a width and a height as result is 1.", "calculate area from and")]
    [InlineData("define factorial of a number as result is 1.", "factorial of")]
    //[InlineData("define the factorial of a number as result is 1.", "the factorial of")]
    [InlineData("To calculate the area of a rectangle: result is 1.", "calculate the area of")]
    [InlineData("Define add and multiply of an A and a B and a C as result is 1.", "add and multiply of and and")]
    [InlineData("To calculate the area of a rectangle (multiplication): result is 1.", "calculate the area of (multiplication)")]
    [InlineData("To calculate the area of a rectangle (multiplication - trivial): result is 1.", "calculate the area of (multiplication - trivial)")]
    [InlineData("to apply some parameters: result is 1.", "apply")]
    [InlineData("to do something with some parameters: result is 1.", "do something with")]
    [InlineData("To put an object at the end of some objects: result is 1.", "put at the end of")]
    [InlineData("to check if the selection is a string: result is 1.", "check if the selection is")]
    public void LabeledWithParameterStatement(string sentence, string marker)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.NotNull(paragraph.Label);
        var labeledStatement = paragraph.Label;
        var statement = Assert.Single(paragraph.Statements);
        Assert.Equal(marker, labeledStatement.Markers.First());
        var resultStatement = Assert.IsType<ResultStatement>(statement);
        Assert.Equal(1, Assert.IsType<IntLiteralExpression>(resultStatement.Value).Value);
    }

    [Theory]
    [InlineData("calculate factorial of a previous number into a previous factorial.", "calculate factorial of")]
    [InlineData("calculate factorial of a previous number into a previous factorial (recursion).", "calculate factorial of (recursion)")]
    [InlineData("calculate factorial of a previous number in the previous factorial (recursion).", "calculate factorial of (recursion)")]
    public void LabeledStatementInvocation(string sentence, string marker)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);

        var invocationStatement = Assert.IsType<InvocationStatement>(statement);
        Assert.Equal(marker, invocationStatement.Marker);

        var parameter = Assert.Single(invocationStatement.Parameters);
        Assert.Equal("previous number", parameter.Name);

        Assert.Equal("previous factorial", invocationStatement.ResultIdentifier.Name);
    }

    [Theory]
    [InlineData("if.")]
    [InlineData("if number.")]
    //[InlineData("if number is 0 then result is 1.")]
    [InlineData("result is a number times factorial of a number minus one.")]
    public void InvalidStatement(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);

        var invocationStatement = Assert.IsType<InvalidStatement>(statement);
    }
    [Theory]
    [InlineData("if a number is 0, if number.")]
    public void ParseInvalidStatementInsideIf(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var statement = Assert.Single(paragraph.Statements);

        var ifStatement = Assert.IsType<IfStatement>(statement);

        var invocationStatement = Assert.IsType<InvalidStatement>(Assert.IsType<BlockStatement>(ifStatement.Then).Statements[0]);
    }

    [Theory]
    [InlineData("to do something else with a parameter identi; to do something a parameter identi: result is 1.", new[] { "do something else with", "do something" })]
    public void MultipleLabelsWithParameterStatement(string sentence, string[] marker)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.NotNull(paragraph.Label);
        var labeledStatement = paragraph.Label;
        var statement = Assert.Single(paragraph.Statements);
        Assert.Equal(marker, labeledStatement.Markers);
        var resultStatement = Assert.IsType<ResultStatement>(statement);
        Assert.Equal(1, Assert.IsType<IntLiteralExpression>(resultStatement.Value).Value);
    }

    [Fact]
    public void EmptyFunction()
    {
        var parseResult = EngLangParser.Parse("to initialize terminal: ");

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        Assert.NotNull(paragraph.Label);
        var labeledStatement = paragraph.Label;
        Assert.Equal("initialize terminal", labeledStatement.Markers.Single());
    }

    [Fact(Skip = "Not implemented yet.")]
    public void EmptyFunctionInsideOtherBlocks()
    {
        var parseResult = EngLangParser.Parse("to prepare things: result is 1. \n\n\nto initialize terminal: \n\n\nto initialize things: result is 1");

        var paragraphs = Assert.IsType<ParagraphList>(parseResult).Paragraphs;
        Assert.Equal(new[] { "prepare things", "initialize terminal", "initialize things" }, paragraphs.SelectMany(p => p.Label.Markers).ToArray());
    }
}
