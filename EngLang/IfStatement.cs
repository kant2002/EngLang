using System.Collections.Generic;

namespace EngLang;

public record IfStatement(Expression Condition, Statement Then, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Condition, Then };
}
