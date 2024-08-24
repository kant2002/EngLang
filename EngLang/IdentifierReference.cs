using System;
using System.Collections.Generic;

namespace EngLang;

public record IdentifierReference(SymbolName Name, SymbolName Type, IdentifierReference? Owner, Yoakke.SynKit.Text.Range Range) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Owner is null ? [Name, Type] : [Name, Type, Owner];
}
