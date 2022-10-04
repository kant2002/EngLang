namespace EngLang;

public record InvocationStatement(string Marker, IdentifierReference[] Parameters, IdentifierReference? ResultIdentifier) : Statement;
