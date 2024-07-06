using System;
using System.Collections.Generic;

namespace EngLang;

public record InvalidExpression(string Code, Yoakke.SynKit.Text.Range Range) : LogicalExpression(LogicalOperator.Invalid, null!, null!, Range)
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
