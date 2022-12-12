using System;
using System.Collections.Generic;

namespace EngLang;

public record IdentifierReference(string Name) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
