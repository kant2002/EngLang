using System;
using System.Collections.Generic;

namespace EngLang;

public record ByteArrayLiteralExpression(byte[] Value) : Expression
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}

