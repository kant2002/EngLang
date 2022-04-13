namespace EngLang
{
    using System.Collections.Generic;

    public class EngLangParseResult
    {
        public List<IdentifierReference> VariableReferences { get; } = new();
        public List<VariableDeclaration> VariableDeclarations { get; } = new();
    }
}