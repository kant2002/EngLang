namespace EngLang
{
    using System;
    using System.Linq;

    public class EngLangParser
    {
        public static SyntaxNode Parse(string sourceCode)
        {
            var parts = sourceCode.Split(' ');
            //var result = new EngLangParseResult();
            switch (parts[0])
            {
                case "a":
                case "an":
                    var variableReference = ParseIdentifierReference(string.Join(' ', parts.Skip(1)));
                    //result.VariableReferences.Add(variableReference);
                    return variableReference;
                    break;
                case "the":
                    var variableDeclaration = ParseVariableDeclaration(string.Join(' ', parts.Skip(1)));
                    //result.VariableDeclarations.Add(variableDeclaration);
                    return variableDeclaration;
                    break;
                default:
                    throw new NotImplementedException();
            }

            //return result;
        }

        private static IdentifierReference ParseIdentifierReference(string content)
        {
            var variableName = content;
            var identifierReference = new IdentifierReference(variableName);
            return identifierReference;
        }

        private static VariableDeclaration ParseVariableDeclaration(string content)
        {
            var parts = content.Split(" is ");
            var variableName = parts[0];
            var typeName = parts[1];
            var syntaxNode = Parse(typeName);
            var identifierReference = syntaxNode as IdentifierReference;
            if (identifierReference == null)
            {
                throw new InvalidOperationException($"Identifier expected. {typeName} given");
            }

            var variableDeclaration = new VariableDeclaration(variableName, identifierReference);
            return variableDeclaration;
        }
    }
}