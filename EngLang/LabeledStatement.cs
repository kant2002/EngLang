using System.Collections.Generic;
using System.Linq;

namespace EngLang;

public record LabeledStatement(InvokableLabel Marker, IdentifierReference[] Parameters, Statement Statement, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => Parameters.Union(new SyntaxNode[] { Statement });
}
