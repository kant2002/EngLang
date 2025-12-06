namespace EngLang.Vm;

public class VmVariableDeclaration
{
    public string Name { get; }
    public VmTypeIdentifierReference Shape { get; }
    public VmVariableDeclaration(string name, VmTypeIdentifierReference shape)
    {
        this.Name = name;
        this.Shape = shape;
    }
}
