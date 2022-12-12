using System.Collections.Generic;

namespace EngLang;

public record VariableExpression(IdentifierReference Identifier) : Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Identifier };
}

