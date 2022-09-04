namespace EngLang;

public record InPlaceSubtractExpression(Expression Subtrahend, IdentifierReference TargetVariable): Expression;
