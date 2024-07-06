using System.Collections.Generic;

namespace EngLang;

public record ShapeDeclarationStatement(ShapeDeclaration Declaration, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Declaration };
}
