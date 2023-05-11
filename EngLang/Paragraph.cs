using System.Collections.Generic;
using System.Collections.Immutable;

namespace EngLang;

public record Paragraph(ImmutableList<Statement> Statements) : Statement
{
    public override IEnumerable<SyntaxNode> Children => Statements;
}
