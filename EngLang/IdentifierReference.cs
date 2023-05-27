using System;
using System.Collections.Generic;

namespace EngLang;

public record IdentifierReference(string Name, IdentifierReference? Owner) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Owner is null ? Array.Empty<SyntaxNode>() : new[] { Owner };
}
