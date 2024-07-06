using System;
using System.Collections.Generic;

namespace EngLang;

public record NullLiteralExpression(Yoakke.SynKit.Text.Range Range): Expression
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
