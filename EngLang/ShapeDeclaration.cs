using System.Collections.Generic;

namespace EngLang;

public record ShapeDeclaration(SymbolName Name, TypeIdentifierReference? BaseShapeName, SlotDeclarationsList? WellKnownSlots = null) : SyntaxNode
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
                foreach (var slot in WellKnownSlots.Slots)
                {
                    yield return slot;
                }
            }
        }
    }
}
