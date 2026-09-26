namespace EngLang.Vm;

using System.Diagnostics;

internal class ExecutionScope
{
    private Dictionary<string, VmVariableDeclaration> variables = new();
    private Dictionary<string, object?> variableValues = new();

    public VmVariableDeclaration? GetVariableDeclaration(string variableName)
    {
        return this.variables.TryGetValue(variableName, out var declaration) ? declaration : null;
    }

    public object GetVariableValue(string variableName)
    {
        return variableValues.TryGetValue(variableName, out var value)
            ? value
            : throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
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
            if (!this.variables.ContainsKey(variableName))
            {
                throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
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
            if (!this.variables.ContainsKey(variableName))
            {
                throw new EngLangRuntimeException($"Variable '{variableName}' is not defined.");
            }
            return this.variableValues.TryGetValue(variableName, out var value) ? value : null;
        });
    }

    public void RegisterVariableValue(string variableName, object? value)
    {
        this.variableValues.Add(variableName, value);
    }
}
