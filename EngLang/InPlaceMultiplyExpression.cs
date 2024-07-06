using System.Collections.Generic;

namespace EngLang;

public record InPlaceMultiplyExpression(Expression Factor, IdentifierReference TargetVariable, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Factor, TargetVariable };
}

