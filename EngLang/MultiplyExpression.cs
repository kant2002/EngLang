namespace EngLang;

public record MultiplyExpression(Expression Factor, IdentifierReference TargetVariable): Expression;
