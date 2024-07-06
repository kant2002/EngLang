namespace EngLang;

public abstract record Statement(Yoakke.SynKit.Text.Range Range) : SyntaxNode;
