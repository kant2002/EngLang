using System;
using System.Collections.Generic;
using System.Text;

namespace EngLang.Vm;

public class VmFunctionDeclaration
{
    public string Name { get; }
    public int ParametersCount { get; }
    public VmFunctionDeclaration(string name, int parametersCount)
    {
        this.Name = name;
        this.ParametersCount = parametersCount;
    }
}
