using System.Collections.Generic;

namespace EngLang;

public record ShapeDeclaration(string Name, TypeIdentifierReference? BaseShapeName, IdentifierReferencesList? WellKnownSlots = null) : SyntaxNode
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
                foreach (var slot in WellKnownSlots.IdentifierReferences)
                {
                    yield return slot;
                }
            }
        }
    }
}
