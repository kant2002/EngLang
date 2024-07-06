using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Yoakke.SynKit.Lexer;

namespace EngLang;

public record InvalidStatement(ImmutableArray<IToken<EngLangTokenType>> Tokens, Yoakke.SynKit.Text.Range Range) : Statement(Range)
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
