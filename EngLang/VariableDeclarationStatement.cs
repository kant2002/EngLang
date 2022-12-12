using System.Collections.Generic;

namespace EngLang;

public record VariableDeclarationStatement(VariableDeclaration Declaration) : Statement
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Declaration };
}
