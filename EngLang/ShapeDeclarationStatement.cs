using System.Collections.Generic;

namespace EngLang;

public record ShapeDeclarationStatement(ShapeDeclaration Declaration) : Statement
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Declaration };
}
