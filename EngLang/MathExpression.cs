using System.Collections.Generic;

namespace EngLang;

public record MathExpression(MathOperator Operator, Expression FirstOperand, Expression SecondOperand): Expression
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { FirstOperand, SecondOperand };
}

