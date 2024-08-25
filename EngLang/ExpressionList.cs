using System.Collections.Immutable;

namespace EngLang;

public record ExpressionList(ImmutableList<Expression> IdentifierReferences)
{
}
