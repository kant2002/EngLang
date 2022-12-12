using System.Collections.Generic;

namespace EngLang;

public record InPlaceDivisionExpression(Expression Denominator, IdentifierReference TargetVariable): Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Denominator, TargetVariable };
}
