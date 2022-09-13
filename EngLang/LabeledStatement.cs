namespace EngLang;

public record LabeledStatement(string Marker, IdentifierReference[] Parameters, Statement Statement) : Statement;
