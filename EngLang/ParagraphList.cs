using System.Collections.Generic;
using System.Collections.Immutable;

namespace EngLang;

public record ParagraphList(ImmutableList<Paragraph> Paragraphs) : SyntaxNode
{
    public override IEnumerable<SyntaxNode> Children => Paragraphs;
}
