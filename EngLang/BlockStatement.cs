using System.Collections.Generic;
using System.Collections.Immutable;

namespace EngLang;

public record BlockStatement(ImmutableList<Statement> Statements, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => Statements;
}
