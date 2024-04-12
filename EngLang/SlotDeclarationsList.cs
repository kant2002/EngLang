using System.Collections.Immutable;

namespace EngLang;

public record SlotDeclarationsList(ImmutableList<SlotDeclaration> Slots)
{
}
