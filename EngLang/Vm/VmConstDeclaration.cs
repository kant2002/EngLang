namespace EngLang.Vm;

public class VmConstDeclaration
{
    public string Name { get; }
    public object? Value { get; }

    public VmConstDeclaration(string name, object? value)
    {
        this.Name = name;
        this.Value = value;
    }
}
