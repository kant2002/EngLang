using EngLang.Vm;
using Xunit;

namespace EngLang.Tests;

public class VirtualMachineTests
{
    [Fact]
    public void NewVariableDeclaration()
    {
        var sentence = "the width is a number.";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableDeclaration = vm.GetVariableDeclaration("width");
        Assert.NotNull(variableDeclaration);
        Assert.Equal("width", variableDeclaration.Name);
        // Should be some enumerable which represent well-known type
        Assert.Equal("number", variableDeclaration.Shape.Type);
    }
    [Fact]
    public void VariableReDeclaration()
    {
        var sentence = "the width is a number.";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);
        try
        {
            vm.ExecuteCode(sentence);
            Assert.Fail("The second variable redeclaration should not happens");
        }
        catch (EngLangRuntimeException ex)
        {
            Assert.Equal("Variable 'width' is already declared.", ex.Message);
        }
    }
}
