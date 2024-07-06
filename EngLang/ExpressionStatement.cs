using System.Collections.Generic;

namespace EngLang;

public record ExpressionStatement(Expression Expression, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Expression };
}
