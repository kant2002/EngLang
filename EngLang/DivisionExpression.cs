namespace EngLang;

public record DivisionExpression(Expression Denominator, IdentifierReference TargetVariable): Expression;
