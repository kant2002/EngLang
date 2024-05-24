using System;
using System.Collections.Generic;

namespace EngLang;

public record IdentifierReference(SymbolName Name, IdentifierReference? Owner, Yoakke.SynKit.Text.Range Range) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Owner is null ? [Name] : [Name, Owner];
}
