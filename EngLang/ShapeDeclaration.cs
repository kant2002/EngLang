using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EngLang;

public record ShapeDeclaration(string Name, IdentifierReference BaseShapeName, ImmutableArray<IdentifierReference>? WellKnownSlots = null) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children =>
        WellKnownSlots is null ? new SyntaxNode[] { BaseShapeName } : new SyntaxNode[] { BaseShapeName }.Concat(WellKnownSlots);
}
