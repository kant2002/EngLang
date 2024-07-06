using System.Collections.Generic;

namespace EngLang;

public record AssignmentExpression(IdentifierReference Variable, Expression Expression, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Variable, Expression };
}
