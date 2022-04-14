using System.Collections.Generic;

namespace EngLang;

public class EngLangParseResult
{
    public List<IdentifierReference> VariableReferences { get; } = new();
    public List<VariableDeclaration> VariableDeclarations { get; } = new();
}