using System.Collections.Generic;
using System.Linq;

namespace EngLang;

public record InvocationStatement(string Marker, Expression[] Parameters, IdentifierReference? ResultIdentifier, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => ResultIdentifier is null ? Parameters : Parameters.Union(new SyntaxNode[] { ResultIdentifier });
}
