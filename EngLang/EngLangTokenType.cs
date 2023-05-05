namespace EngLang;

using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;

public enum EngLangTokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex("([ \t]+((\r\n|\n|\r)[ \t]*)?|(\r\n|\n|\r))")] Whitespace,
    [Regex("(\r\n|\n|\r){2,}")] Multiline,

    [Regex("(a|an)")] IndefiniteArticleKeyword,
    [Token("the")] DefiniteArticleKeyword,

    [Token(";")] SemicolonKeyword,
    [Token(".")] DotKeyword,

    [Regex("(add|subtract|multiply|multiplied|divide|divided|plus|minus)")] MathOperationKeyword,

    [Regex("(smaller|bigger|less|greater)")] LogicalOperationKeyword,

    [Token("put")] PutKeyword,
    [Token("let")] LetKeyword,

    [Token("if")] IfKeyword,
    [Token("is")] IsKeyword,
    [Regex("(equal|equals)")] EqualKeyword,
    [Token("into")] IntoKeyword,
    [Token("by")] ByKeyword,
    [Token("and")] AndKeyword,
    [Regex("(then|,)")] ThenKeyword,

    //[Regex("(\\?!\\band\\b)([A-Za-z_][A-Za-z0-9_]+)(\\?<!\\band\\b)")] Identifier,
    [Regex(Regexes.Identifier)] Identifier,
    [Regex(Regexes.IntLiteral)] IntLiteral,
    [Regex(Regexes.StringLiteral)] StringLiteral,
    [Token("null")] NullLiteral,
}
