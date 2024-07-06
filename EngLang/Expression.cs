namespace EngLang;

public abstract record Expression(Yoakke.SynKit.Text.Range Range) : SyntaxNode;
