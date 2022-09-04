namespace EngLang;

public record InPlaceDivisionExpression(Expression Denominator, IdentifierReference TargetVariable): Expression;
