using System.Collections.Generic;

namespace EngLang;

public record AssignmentExpression(IdentifierReference Variable, Expression Expression) : Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Variable, Expression };
}
