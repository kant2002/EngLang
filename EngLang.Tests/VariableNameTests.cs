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
            var variableDeclaration = parseResult.VariableDeclarations[0];
            Assert.Equal(expectedVariableName, variableDeclaration.Name);
        }

        [Fact]
        public void DetectVariableWithWhiteSpaces()
        {
            var sentence = "a red apple";

            var parseResult = EngLangParser.Parse(sentence);

            var expectedVariableName = "red apple";
            var variableDeclaration = parseResult.VariableDeclarations[0];
            Assert.Equal(expectedVariableName, variableDeclaration.Name);
        }
    }
}
