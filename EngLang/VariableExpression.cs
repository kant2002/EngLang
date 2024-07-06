using System.Collections.Generic;

namespace EngLang;

public record VariableExpression(IdentifierReference Identifier, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Identifier };
}

