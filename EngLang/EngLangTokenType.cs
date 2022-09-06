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

    [Token(";")] SemicolonKeyword,
    [Token(".")] DotKeyword,

    [Regex("(add|subtract|multiply|divide|plus|minus)")] MathOperationKeyword,

    [Regex("(smaller|bigger|less|greater)")] LogicalOperationKeyword,

    [Token("put")] PutKeyword,
    [Token("let")] LetKeyword,

    [Token("if")] IfKeyword,
    [Token("is")] IsKeyword,
    [Regex("(equal|equals)")] EqualKeyword,
    [Token("into")] IntoKeyword,
    [Token("by")] ByKeyword,

    [Regex(Regexes.Identifier)] Identifier,
    [Regex(Regexes.IntLiteral)] IntLiteral,
    [Regex(Regexes.StringLiteral)] StringLiteral,
}
