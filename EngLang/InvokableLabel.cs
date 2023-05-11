using System;
using System.Collections.Generic;

namespace EngLang;

public record InvokableLabel(string Marker, IdentifierReference[] Parameters) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
