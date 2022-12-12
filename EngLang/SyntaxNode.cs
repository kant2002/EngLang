using System.Collections.Generic;

namespace EngLang;

public abstract record SyntaxNode()
{
    public abstract IEnumerable<SyntaxNode> Children { get; }
}
