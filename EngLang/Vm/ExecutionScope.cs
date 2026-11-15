namespace EngLang.Vm;

using System.Diagnostics;

internal class ExecutionScope
{
    private Dictionary<string, VmVariableDeclaration> variables = new();
    private Dictionary<string, object?> variableValues = new();
    private ExecutionScope? parentScope;
    public ExecutionScope()
    {
    }
    public ExecutionScope(ExecutionScope parentScope)
    {
        this.parentScope = parentScope;
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
            if (!this.variables.ContainsKey(variableName) && declaration is not null)
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
}
