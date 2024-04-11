namespace EngLang;

using System;
using System.Collections.Generic;

public record TypeIdentifierReference(string Name, bool IsCollection) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
