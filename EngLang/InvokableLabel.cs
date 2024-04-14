using System;
using System.Collections.Generic;

namespace EngLang;

public record InvokableLabel(string[] Markers, IdentifierReference[] Parameters, IdentifierReference? ResultIdentifier) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
