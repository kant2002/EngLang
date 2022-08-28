namespace EngLang;

public record SubtractExpression(Expression Subtrahend, IdentifierReference TargetVariable): Expression;
