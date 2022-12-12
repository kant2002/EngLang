using System.Collections.Generic;

namespace EngLang;

public record ResultStatement(Expression Value) : Statement
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Value };
}
