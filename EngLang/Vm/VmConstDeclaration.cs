namespace EngLang.Vm;

public class VmConstDeclaration
{
    public string Name { get; }
    public VmValue? Value { get; }

    public VmConstDeclaration(string name, VmValue? value)
    {
        this.Name = name;
        this.Value = value;
    }
}
