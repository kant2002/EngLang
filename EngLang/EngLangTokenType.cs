namespace EngLang;

using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;

public enum EngLangTokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex("([ \t]+((\r\n|\n|\r)[ \t]*)?|(\r\n|\n|\r))")] Whitespace,
    [Regex("(\r\n|\n|\r){2,}")] Multiline,
    [Ignore] [Regex("\\\\[ \ta-zA-Z0-9,():.\\-/+=]*")] Comment,

    [Regex("(a|an|An)")] IndefiniteArticleKeyword,
    [Token("some")] SomeKeyword,
    [Token("the")] DefiniteArticleKeyword,

    [Token(";")] SemicolonKeyword,
    [Token(".")] DotKeyword,
    [Regex("(->|as)")] FunctionBodyKeyword,

    [Regex("(add|subtract|multiply|multiplied|divide|divided|plus|minus)")] MathOperationKeyword,

    [Regex("(smaller|bigger|less|greater)")] LogicalOperationKeyword,

    [Token("put")] PutKeyword,
    [Token("let")] LetKeyword,

    [Regex("(if|If)")] IfKeyword,
    [Token("is")] IsKeyword,
    [Token("are")] AreKeyword,
    [Token("has")] HasKeyword,
    [Regex("(equal|equals)")] EqualKeyword,
    [Regex("(in|into)")] IntoKeyword,
    [Token("by")] ByKeyword,
    [Token("and")] AndKeyword,
    [Token("with")] WithKeyword,
    [Token("of")] OfKeyword,
    [Regex("then")] ThenKeyword,
    [Regex(",")] CommaKeyword,
    [Regex("(null|nil)")] NullLiteral,

    //[Regex("(\\?!\\band\\b)([A-Za-z_][A-Za-z0-9_]+)(\\?<!\\band\\b)")] Identifier,
    //[Regex(Regexes.Identifier)]
    [Regex("[A-Za-z_]([A-Za-z0-9_\\-]*[A-Za-z_]|[A-Za-z_]?)('s)?")]
    Identifier,
    [Regex(Regexes.IntLiteral)] IntLiteral,
    [Regex(Regexes.StringLiteral)] StringLiteral,
}
