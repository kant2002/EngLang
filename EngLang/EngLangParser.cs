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
            switch (parts[0])
            {
                case "a":
                case "an":
                    ParseVariableReference(result, string.Join(' ', parts.Skip(1)));
                    break;
                case "the":
                    ParseVariableDeclaration(result, string.Join(' ', parts.Skip(1)));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private static void ParseVariableReference(EngLangParseResult result, string content)
        {
            var variableName = content;
            var variableReference = new VariableReference(variableName);
            result.VariableReferences.Add(variableReference);
        }

        private static void ParseVariableDeclaration(EngLangParseResult result, string content)
        {
            var parts = content.Split(" is a ");
            var variableName = parts[0];
            var typeName = parts[1].Trim('.');
            var variableDeclaration = new VariableDeclaration(variableName, typeName);
            result.VariableDeclarations.Add(variableDeclaration);
        }
    }
}