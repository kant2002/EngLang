using System.Collections.Generic;

namespace EngLang;

public record ConstantDeclarationStatement(IdentifierReference Identifier, Expression Value, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => [Identifier, Value];
}
