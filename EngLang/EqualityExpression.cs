namespace EngLang;

public record EqualityExpression(IdentifierReference Variable, Expression Expression) : Expression;
