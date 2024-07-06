using System.Collections.Generic;

namespace EngLang;

public record InPlaceDivisionExpression(Expression Denominator, IdentifierReference TargetVariable, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Denominator, TargetVariable };
}
