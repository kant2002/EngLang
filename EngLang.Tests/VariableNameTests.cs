using Xunit;

namespace EngLang.Tests;

public class VariableNameTests
{
    [Fact]
    public void DetectVariable()
    {
        var sentence = "an apple";

        var parseResult = EngLangParser.Parse(sentence);

        var expectedVariableName = "apple";
        var variableDeclaration = Assert.IsType<IdentifierReference>(parseResult);
        Assert.Equal(expectedVariableName, variableDeclaration.Name);
    }

    [Fact]
    public void DetectVariableWithWhiteSpaces()
    {
        var sentence = "a red apple";

        var parseResult = EngLangParser.Parse(sentence);

        var expectedVariableName = "red apple";
        var variableDeclaration = Assert.IsType<IdentifierReference>(parseResult);
        Assert.Equal(expectedVariableName, variableDeclaration.Name);
    }

    [Fact]
    public void WhitespacesInVariableNameNormalized()
    {
        var sentence = "a red   apple";

        var parseResult = EngLangParser.Parse(sentence);

        var expectedVariableName = "red apple";
        var variableDeclaration = Assert.IsType<IdentifierReference>(parseResult);
        Assert.Equal(expectedVariableName, variableDeclaration.Name);
    }

    [Fact]
    public void VariableNameCanHaveDash()
    {
        var sentence = "a zero-indexed";

        var parseResult = EngLangParser.Parse(sentence);

        var expectedVariableName = "zero-indexed";
        var variableDeclaration = Assert.IsType<IdentifierReference>(parseResult);
        Assert.Equal(expectedVariableName, variableDeclaration.Name);
    }

    [Fact]
    public void DeclareVariableWithType()
    {
        var sentence = "the name is a string";

        var parseResult = EngLangParser.Parse(sentence);

        var expectedVariableName = "name";
        var variableDeclaration = Assert.IsType<VariableDeclaration>(parseResult);
        Assert.Equal(expectedVariableName, variableDeclaration.Name);
        Assert.Equal("string", variableDeclaration.TypeName.Name);
    }

    [Fact]
    public void DeclareVariableWithAnType()
    {
        var sentence = "the name is an apple";

        var parseResult = EngLangParser.Parse(sentence);

        var variableDeclaration = Assert.IsType<VariableDeclaration>(parseResult);
        Assert.Equal("name", variableDeclaration.Name);
        Assert.Equal("apple", variableDeclaration.TypeName.Name);
        Assert.False(variableDeclaration.TypeName.IsCollection);
        Assert.Null(variableDeclaration.Expression);
    }

    [Fact]
    public void DeclareCollectionVariableWithAnType()
    {
        var sentence = "the apples are some fruits";

        var parseResult = EngLangParser.Parse(sentence);

        var variableDeclaration = Assert.IsType<VariableDeclaration>(parseResult);
        Assert.Equal("apples", variableDeclaration.Name);
        Assert.Equal("fruit", variableDeclaration.TypeName.Name);
        Assert.True(variableDeclaration.TypeName.IsCollection);
        Assert.Null(variableDeclaration.Expression);
    }

    [Fact]
    public void DeclareVariableWithAnStringLiteralInitializer()
    {
        var sentence = "the greetings is an string equal to \"Hello\"";

        var parseResult = EngLangParser.Parse(sentence);

        var variableDeclaration = Assert.IsType<VariableDeclaration>(parseResult);
        Assert.Equal("greetings", variableDeclaration.Name);
        Assert.Equal("string", variableDeclaration.TypeName.Name);
        var stringLiteralExpression = Assert.IsType<StringLiteralExpression>(variableDeclaration.Expression);
        Assert.Equal("Hello", stringLiteralExpression.Value);
    }

    [Fact]
    public void DeclareVariableWithAnIntLiteralInitializer()
    {
        var sentence = "the answer to all things is an number equal to 42";

        var parseResult = EngLangParser.Parse(sentence);

        var variableDeclaration = Assert.IsType<VariableDeclaration>(parseResult);
        Assert.Equal("answer to all things", variableDeclaration.Name);
        Assert.Equal("number", variableDeclaration.TypeName.Name);
        var intLiteralExpression = Assert.IsType<IntLiteralExpression>(variableDeclaration.Expression);
        Assert.Equal(42, intLiteralExpression.Value);
    }

    [Fact]
    public void ParseVariableDeclarationSatement()
    {
        var sentence = "the name is an apple.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);
        var variableStatement = Assert.IsType<VariableDeclarationStatement>(Assert.Single(paragraph.Statements));
        Assert.Equal("name", variableStatement.Declaration.Name);
        Assert.Equal("apple", variableStatement.Declaration.TypeName.Name);
        Assert.Null(variableStatement.Declaration.Expression);
    }

    [Fact]
    public void ParseVariableAssignment()
    {
        var sentence = "put 40 into a value.";

        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);

        var assignmentStatement = Assert.IsType<ExpressionStatement>(Assert.Single(paragraph.Statements));
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
        var expression = Assert.IsType<IntLiteralExpression>(assignmentExpression.Expression);
        Assert.Equal(40, expression.Value);
    }

    [Theory]
    [InlineData("put 40 into a value. \\ 123")]
    [InlineData("put 40 into a value. \\ your's")]
    public void ParseComment(string sentence)
    {
        var parseResult = EngLangParser.Parse(sentence);

        var paragraph = Assert.Single(Assert.IsType<ParagraphList>(parseResult).Paragraphs);

        var assignmentStatement = Assert.IsType<ExpressionStatement>(Assert.Single(paragraph.Statements));
        var assignmentExpression = Assert.IsType<AssignmentExpression>(assignmentStatement.Expression);
        Assert.Equal("value", assignmentExpression.Variable.Name);
        var expression = Assert.IsType<IntLiteralExpression>(assignmentExpression.Expression);
        Assert.Equal(40, expression.Value);
    }
}
