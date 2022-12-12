using System.Collections.Generic;

namespace EngLang;

public record ExpressionStatement(Expression Expression) : Statement
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Expression };
}
