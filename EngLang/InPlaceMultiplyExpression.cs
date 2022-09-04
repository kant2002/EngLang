namespace EngLang;

public record InPlaceMultiplyExpression(Expression Factor, IdentifierReference TargetVariable): Expression;
