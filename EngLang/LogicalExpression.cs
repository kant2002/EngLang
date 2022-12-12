using System.Collections.Generic;

namespace EngLang;

public record LogicalExpression(LogicalOperator Operator, Expression FirstOperand, Expression SecondOperand) : Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { FirstOperand, SecondOperand };
}

