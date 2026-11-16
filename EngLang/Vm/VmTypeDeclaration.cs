namespace EngLang.Vm;

public class VmTypeDeclaration
{
    public string Name { get; }
    public Dictionary<string, VmTypeDeclaration> Properties { get; } = new Dictionary<string, VmTypeDeclaration>();
    public VmTypeDeclaration(string name)
    {
        this.Name = name;
    }
}
