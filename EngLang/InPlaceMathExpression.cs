using System.Collections.Generic;

namespace EngLang;

public record InPlaceMathExpression(MathOperator Operator, Expression ChangeValue, IdentifierReference TargetVariable, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { ChangeValue, TargetVariable };
}
