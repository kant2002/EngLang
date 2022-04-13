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
            var variableReference = parseResult.VariableReferences[0];
            Assert.Equal(expectedVariableName, variableReference.Name);
        }
        [Fact]
        public void DetectVariableWithWhiteSpaces()
        {
            var sentence = "an red apple";

            var parseResult = EngLangParser.Parse(sentence);

            var expectedVariableName = "red apple";
            var variableReference = parseResult.VariableReferences[0];
            Assert.Equal(expectedVariableName, variableReference.Name);
        }
    }
}
