using System.Collections.Generic;

namespace EngLang;

public record InPlaceSubtractExpression(Expression Subtrahend, IdentifierReference TargetVariable): Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Subtrahend, TargetVariable };
}

