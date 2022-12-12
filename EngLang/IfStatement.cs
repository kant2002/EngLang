using System.Collections.Generic;

namespace EngLang;

public record IfStatement(Expression Condition, Statement Then) : Statement
{
    public override IEnumerable<SyntaxNode> Children => new SyntaxNode[] { Condition, Then };
}
