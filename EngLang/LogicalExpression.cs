namespace EngLang;

public record LogicalExpression(LogicalOperator Operator, Expression FirstOperand, Expression SecondOperand) : Expression;
