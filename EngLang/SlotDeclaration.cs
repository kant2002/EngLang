using System;
using System.Collections.Generic;

namespace EngLang;

public record SlotDeclaration(string Name, string TypeName, CommentLabel? Comment, bool IsCollection, int? CollectionSize, string? AliasFor, Yoakke.SynKit.Text.Range Range) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
