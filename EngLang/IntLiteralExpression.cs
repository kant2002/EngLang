using System;
using System.Collections.Generic;

namespace EngLang;

public record IntLiteralExpression(int Value) : Expression
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}

