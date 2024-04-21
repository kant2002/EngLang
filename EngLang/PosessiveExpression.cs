using System.Collections.Generic;

namespace EngLang;

public record PosessiveExpression(IdentifierReference Identifier, Expression Owner) : Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Identifier, Owner };
}
