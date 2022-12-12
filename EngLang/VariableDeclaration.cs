using System.Collections.Generic;

namespace EngLang;

public record VariableDeclaration(string Name, IdentifierReference TypeName, Expression? Expression = null) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children =>
        Expression is null ? new SyntaxNode[] { TypeName } : new SyntaxNode[] { TypeName, Expression };
}
