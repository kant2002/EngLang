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

        var blockStatement = Assert.IsType<BlockStatement>(parseResult);
        Assert.Single(blockStatement.Statements);

        var variableStatement = Assert.IsType<VariableDeclarationStatement>(blockStatement.Statements[0]);
        Assert.Equal("name", variableStatement.Declaration.Name);
        Assert.Equal("apple", variableStatement.Declaration.TypeName.Name);
        Assert.Null(variableStatement.Declaration.Expression);
    }
}
