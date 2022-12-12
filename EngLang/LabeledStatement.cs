using System.Collections.Generic;
using System.Linq;

namespace EngLang;

public record LabeledStatement(string Marker, IdentifierReference[] Parameters, Statement Statement) : Statement
{
    public override IEnumerable<SyntaxNode> Children => Parameters.Union(new SyntaxNode[] { Statement });
}
