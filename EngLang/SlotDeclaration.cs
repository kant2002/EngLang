using System;
using System.Collections.Generic;

namespace EngLang;

public record SlotDeclaration(string Name, bool IsCollection, string? AliasFor) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
