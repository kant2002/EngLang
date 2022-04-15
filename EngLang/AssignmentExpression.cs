namespace EngLang;

public record AssignmentExpression(IdentifierReference Variable, Expression Expression): Expression;
