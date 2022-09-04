namespace EngLang;

using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;

public enum EngLangTokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex("[ \t\r\n]+")] Whitespace,

    [Regex("(a|an)")] IndefiniteArticleKeyword,
    [Token("the")] DefiniteArticleKeyword,

    [Regex("(add|subtract|multiply|divide)")] MathOperationKeyword,

    [Token("put")] PutKeyword,

    [Token("if")] IfKeyword,
    [Token("is")] IsKeyword,
    [Token("equal")] EqualKeyword,
    [Token("into")] IntoKeyword,
    [Token("by")] ByKeyword,

    [Regex(Regexes.Identifier)] Identifier,
    [Regex(Regexes.IntLiteral)] IntLiteral,
    [Regex(Regexes.StringLiteral)] StringLiteral,
}
