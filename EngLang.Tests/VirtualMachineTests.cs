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
    [Fact]
    public void NewVariableDeclarationWithIntInitializer()
    {
        var sentence = "the width is a number equals to 5.";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("width");
        Assert.Equal(5, (long)variableValue);
    }
    [Fact]
    public void NewVariableDeclarationWithStringInitializer()
    {
        var sentence = "the greeting is a string equals to \"Hello\".";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("greeting");
        Assert.Equal("Hello", (string)variableValue);
    }
    [Fact]
    public void ImplmicitVariableDeclaration()
    {
        var sentence = "let a value equals 10.";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(10, (long)variableValue);
    }
    [Fact]
    public void Addition()
    {
        var sentence = "let a value equals 10. add 20 to a value.";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(30, (long)variableValue);
    }
}
