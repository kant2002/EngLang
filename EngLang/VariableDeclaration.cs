namespace EngLang;

public record VariableDeclaration(string Name, IdentifierReference TypeName, Expression? Expression = null) : SyntaxNode;