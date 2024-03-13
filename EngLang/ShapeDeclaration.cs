using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EngLang;

public record ShapeDeclaration(string Name, IdentifierReference? BaseShapeName, ImmutableArray<IdentifierReference>? WellKnownSlots = null) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            yield return new IdentifierReference(Name, null);
            if (BaseShapeName is not null)
                yield return BaseShapeName;

            if (WellKnownSlots is not null)
            {
                foreach (var slot in WellKnownSlots)
                {
                    yield return slot;
                }
            }
        }
    }
}
