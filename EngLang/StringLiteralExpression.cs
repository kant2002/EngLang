using System;
using System.Collections.Generic;

namespace EngLang;

public record StringLiteralExpression(string Value) : Expression
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}

