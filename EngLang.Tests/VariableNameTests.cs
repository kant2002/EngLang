using System;
using Xunit;

namespace EngLang.Tests
{
    public class VariableNameTests
    {
        [Fact]
        public void Test1()
        {
            var sentence = "an apple";

            var parseResult = EngLangParser.Parse(sentence);

            var expectedVariableName = "apple";
            var variableReference = parseResult.VariableReferences[0];
            Assert.Equal(expectedVariableName, variableReference.Name);
        }
    }
}
