using System.Collections.Generic;

namespace EngLang;

public record ShapeDeclaration(SymbolName Name, TypeIdentifierReference? BaseShapeName, SlotDeclarationsList? WellKnownSlots, Yoakke.SynKit.Text.Range Range) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            yield return new IdentifierReference(Name, Name, null, Name.Range);
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
