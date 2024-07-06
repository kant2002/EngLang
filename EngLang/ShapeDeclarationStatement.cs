using System.Collections.Generic;

namespace EngLang;

public record ShapeDeclarationStatement(ShapeDeclaration Declaration, Yoakke.SynKit.Text.Range Range) : Statement
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Declaration };
}
