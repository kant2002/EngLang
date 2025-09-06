using System.Collections.Generic;

namespace EngLang;

public record PointerDeclarationStatement(IdentifierReference PointerType, TypeIdentifierReference BaseType, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => [];
}
