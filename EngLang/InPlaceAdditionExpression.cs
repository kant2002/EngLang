using System.Collections.Generic;

namespace EngLang;

public record InPlaceAdditionExpression(Expression Addend, IdentifierReference TargetVariable): Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Addend, TargetVariable };
}
