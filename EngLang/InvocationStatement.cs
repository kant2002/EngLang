using System.Collections.Generic;
using System.Linq;

namespace EngLang;

public record InvocationStatement(string Marker, IdentifierReference[] Parameters, IdentifierReference? ResultIdentifier) : Statement
{
    public override IEnumerable<SyntaxNode> Children => ResultIdentifier is null ? Parameters : Parameters.Union(new SyntaxNode[] { ResultIdentifier });
}
