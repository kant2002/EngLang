using System.Collections.Generic;

namespace EngLang;

public record InPlaceAdditionExpression(Expression Addend, IdentifierReference TargetVariable, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Addend, TargetVariable };
}
