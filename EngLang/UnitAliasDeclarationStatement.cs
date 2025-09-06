using System.Collections.Generic;

namespace EngLang;

public record UnitAliasDeclarationStatement(IdentifierReference Identifier, Expression Value, IdentifierReference BaseUnit, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => [Identifier, Value, BaseUnit];
}
