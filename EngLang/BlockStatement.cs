using System.Collections.Generic;
using System.Collections.Immutable;

namespace EngLang;

public record BlockStatement(ImmutableList<Statement> Statements) : Statement
{
    public override IEnumerable<SyntaxNode> Children => Statements;
}
