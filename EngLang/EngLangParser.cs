namespace EngLang
{
    using System;
    using System.Linq;

    public class EngLangParser
    {
        public static EngLangParseResult Parse(string sourceCode)
        {
            var parts = sourceCode.Split(' ');
            var result = new EngLangParseResult();
            if (parts[0] == "an")
            {
                var variableName = string.Join(' ', parts.Skip(1));
                result.VariableReferences.Add(new VariableReference() { Name = variableName });
            }

            return result;
        }
    }
}