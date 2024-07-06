using System;
using System.Collections.Generic;

namespace EngLang;

public record StringLiteralExpression(string Value, Yoakke.SynKit.Text.Range Range) : Expression
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}

