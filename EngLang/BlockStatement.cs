using System.Collections.Immutable;

namespace EngLang;

public record BlockStatement(ImmutableList<Statement> Statements): Statement;
