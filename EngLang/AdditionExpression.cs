namespace EngLang;

public record AdditionExpression(Expression Addend, IdentifierReference TargetVariable): Expression;
