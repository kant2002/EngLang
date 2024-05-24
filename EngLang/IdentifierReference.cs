using System;
using System.Collections.Generic;

namespace EngLang;

public record IdentifierReference(SymbolName Name, IdentifierReference? Owner) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Owner is null ? [Name] : [Name, Owner];
}
