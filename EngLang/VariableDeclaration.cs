namespace EngLang;

public record VariableDeclaration(string Name, IdentifierReference TypeName) : SyntaxNode;