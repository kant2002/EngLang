using System;
using System.Collections.Generic;

namespace EngLang;

public record InvalidExpression(string Code) : LogicalExpression(LogicalOperator.Invalid, null!, null!)
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
