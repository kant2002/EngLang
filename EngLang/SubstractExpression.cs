namespace EngLang;

public record SubstractExpression(Expression Subtrahend, IdentifierReference TargetVariable): Expression;
