using System;
using System.Collections.Generic;

namespace EngLang;

public record InvokableLabel(string[] Markers, IdentifierReference[] Parameters, IdentifierReference? ResultIdentifier, Yoakke.SynKit.Text.Range Range) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
