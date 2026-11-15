using System;
using System.Collections.Generic;
using System.Text;

namespace EngLang.Vm;

public class VmFunctionDeclaration
{
    public string Name { get; }
    public int ParametersCount { get; }
    public List<Statement> Statements { get; }
    public IdentifierReference[] Parameters { get; }

    public VmFunctionDeclaration(string name, IdentifierReference[] parameters, List<Statement> statements)
    {
        this.Name = name;
        this.ParametersCount = parameters.Length;
        this.Parameters = parameters;
        this.Statements = statements;
    }
}
