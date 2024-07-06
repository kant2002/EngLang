using System.Collections.Generic;

namespace EngLang;

public record ResultStatement(Expression Value, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Value };
}
