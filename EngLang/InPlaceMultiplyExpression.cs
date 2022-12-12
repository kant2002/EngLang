using System.Collections.Generic;

namespace EngLang;

public record InPlaceMultiplyExpression(Expression Factor, IdentifierReference TargetVariable): Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Factor, TargetVariable };
}

