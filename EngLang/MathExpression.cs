namespace EngLang;

public record MathExpression(MathOperator Operator, Expression FirstOperand, Expression SecondOperand): Expression;
