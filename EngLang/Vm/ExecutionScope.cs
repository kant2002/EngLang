namespace EngLang.Vm;

using System.Diagnostics;

internal class ExecutionScope
{
    private Dictionary<string, VmVariableDeclaration> variables = new();
    private Dictionary<string, object?> variableValues = new();
    private Dictionary<string, VmConstDeclaration> consts = new();
    private ExecutionScope? parentScope;
    public ExecutionScope()
    {
    }
    public ExecutionScope(ExecutionScope parentScope)
    {
        this.parentScope = parentScope;
    }

    public string[] GetVariableNames()
    {
        var variableNames = new List<string>(this.variables.Keys);
        if (this.parentScope is not null)
        {
            variableNames.AddRange(this.parentScope.GetVariableNames());
        }
        return [.. variableNames];
    }

    public string[] GetConstNames()
    {
        var constNames = new List<string>(this.consts.Keys);
        if (this.parentScope is not null)
        {
            constNames.AddRange(this.parentScope.GetConstNames());
        }
        return [.. constNames];
    }

    public VmVariableDeclaration? GetVariableDeclaration(string variableName)
    {
        return this.variables.TryGetValue(variableName, out var declaration)
            ? declaration
            : this.parentScope?.GetVariableDeclaration(variableName);
    }

    public object? GetVariableValue(string variableName)
    {
        return variableValues.TryGetValue(variableName, out var value)
            ? value
            : this.parentScope is null
                ? throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.")
                : this.parentScope.GetVariableValue(variableName);
    }

    public void RegisterVariable(string variableName, VmTypeIdentifierReference variableType)
    {
        this.variables.Add(variableName, new VmVariableDeclaration(variableName, variableType));
    }

    public Action<object?> GetVariableSetter(IdentifierReference variable)
    {
        Debug.Assert(variable.Owner is null, "Variable owners are not yet supported.");
        Debug.Assert(variable.Type is null || variable.Type.Name == variable.Name.Name, "Variable types is not supported");
        var variableName = variable.Name.Name;
        return (Action<object?>)(value =>
        {
            var declaration = this.GetVariableDeclaration(variableName);
            if (!this.variables.ContainsKey(variableName))
            {
                if (declaration is not null)
                {
                    if (parentScope is null)
                    {
                        throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
                    }
                    else
                    {
                        parentScope.GetVariableSetter(variable)(value);
                        return;
                    }
                }
                else
                {
                    this.variables.Add(variableName, new VmVariableDeclaration(variableName, new VmTypeIdentifierReference() { Type = variable.Type?.Name ?? variableName }));
                }
            }
            this.variableValues[variableName] = value;
        });
    }

    public Func<object?> GetVariableGetter(IdentifierReference variable)
    {
        Debug.Assert(variable.Owner is null, "Variable owners are not yet supported.");
        Debug.Assert(variable.Type is null || variable.Type.Name == variable.Name.Name, "Variable types is not supported");
        var variableName = variable.Name.Name;
        return (Func<object?>)(() =>
        {
            var declaration = this.GetVariableDeclaration(variableName);
            if (!this.variables.ContainsKey(variableName) && declaration is not null)
            {
                if (parentScope is null)
                {
                    throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
                }
                else
                {
                    return parentScope.GetVariableGetter(variable)();
                }
            }

            return this.variableValues.TryGetValue(variableName, out var value) ? value : null;
        });
    }

    public void RegisterVariableValue(string variableName, object? value)
    {
        this.variableValues.Add(variableName, value);
    }

    public VmConstDeclaration GetConstDeclaration(string constName)
    {
        return this.consts.TryGetValue(constName, out var declaration)
            ? declaration
            : this.parentScope is null
                ? throw new EngLangRuntimeException($"Const '{constName}' is not defined.")
                : this.parentScope.GetConstDeclaration(constName);
    }

    public void RegisterConst(string constName, object? value)
    {
        this.consts.Add(constName, new VmConstDeclaration(constName, value));
    }
}
