using System;
using System.Collections.Generic;

namespace EngLang;

public record InchLiteralExpression(int Value, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}

