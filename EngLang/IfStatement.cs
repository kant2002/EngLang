namespace EngLang;

public record IfStatement(Expression Condition, Statement Then) : Statement;
