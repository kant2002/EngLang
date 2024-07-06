using System.Collections.Generic;

namespace EngLang;

public record MathExpression(MathOperator Operator, Expression FirstOperand, Expression SecondOperand, Yoakke.SynKit.Text.Range Range) : Expression(Range)
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { FirstOperand, SecondOperand };
}

