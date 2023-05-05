using System;
using System.Collections.Generic;

namespace EngLang;

public record NullLiteralExpression: Expression
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
