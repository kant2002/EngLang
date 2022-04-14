using System;
using Xunit;

namespace EngLang.Tests
{
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
        }
    }
}
