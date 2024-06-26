namespace EngLang;

using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;

public enum EngLangTokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex("([ \t]+((\r\n|\n|\r)[ \t]*)?|(\r\n|\n|\r))")] Whitespace,
    [Regex("(\r\n|\n|\r){2,}")] Multiline,
    [Ignore][Regex("\\\\[^\n]*")] Comment,
    [Ignore][Regex("\\[[^\\]]*\\]")] InlineComment,

    [Regex("(a|an|An|A|Another|another)")] IndefiniteArticleKeyword,
    [Token("some")] SomeKeyword,
    [Token("the")] DefiniteArticleKeyword,

    [Token(";")] SemicolonKeyword,
    [Token(".")] DotKeyword,
    [Regex("->")] FunctionBodyKeyword,

    [Regex("(add|subtract|multiply|multiplied|divide|divided|plus|minus|-|\\+|/|\\*)")] MathOperationKeyword,

    [Regex("(smaller|bigger|less|greater|most|least)")] LogicalOperationKeyword,

    [Token("put")] PutKeyword,
    [Token("let")] LetKeyword,

    [Regex("(if|If)")] IfKeyword,
    [Token("is")] IsKeyword,
    [Token("at")] AtKeyword,
    [Regex("(to|To)")] ToKeyword,
    [Token("as")] FunctionBodyOrAsKeyword,
    [Token("on")] OnKeyword,
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
    [Regex("from")] FromKeyword,
    [Regex("(null|nil)")] NullLiteral,

    //[Regex("(\\?!\\band\\b)([A-Za-z_][A-Za-z0-9_]+)(\\?<!\\band\\b)")] Identifier,
    //[Regex(Regexes.Identifier)]
    [Regex("[A-Za-z_]([A-Za-z0-9_\\-/':]*[A-Za-z0-9_']|[A-Za-z0-9_']?)#?")]
    Identifier,
    [Regex(Regexes.IntLiteral)] IntLiteral,
    [Regex("(0x|$)[0-9A-Fa-f]+")] HexLiteral,
    [Regex("\"((\"\")|[^\\r\\n\"])*\"")] StringLiteral,
    [Regex("-?[0-9]+°(-?[0-9]+’)?(-?[0-9.]+”)?(-?[0-9.]+cas)?")] TemperatureLiteral,
    [Regex("('s|')")] PosessiveKeyword,
}
