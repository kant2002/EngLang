using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Yoakke.SynKit.Lexer;

namespace EngLang;

public record InvalidStatement(ImmutableArray<IToken<EngLangTokenType>> Tokens) : Statement
{
    public override IEnumerable<SyntaxNode> Children => Array.Empty<SyntaxNode>();
}
