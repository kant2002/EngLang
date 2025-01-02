using System.Collections.Generic;

namespace EngLang;

public record InvocationExpression(string Marker, Expression[] Parameters, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => Parameters;
}
