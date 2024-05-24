using System;
using System.Collections.Generic;

namespace EngLang;

public record CommentLabel(string Text, Yoakke.SynKit.Text.Range Range) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
