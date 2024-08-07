using System.Collections.Generic;
using System.Collections.Immutable;

namespace EngLang;

public record Paragraph(ImmutableList<Statement> Statements, InvokableLabel? Label, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            if (Label is not null)
                yield return Label;

            foreach (var s in Statements)
            {
                yield return s;
            }
        }
    }
}
