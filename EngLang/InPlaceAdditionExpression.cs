namespace EngLang;

public record InPlaceAdditionExpression(Expression Addend, IdentifierReference TargetVariable): Expression;
