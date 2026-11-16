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
    [Fact]
    public void Multiply()
    {
        var sentence = "let a value equals 10. multiply a value by 42.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(420, (long)variableValue);
    }
    [Fact]
    public void ConditionalExpressionTrueCase()
    {
        var sentence = "let a value equals 10. if a value is 10 then multiply a value by 42.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(420, (long)variableValue);
    }
    [Fact]
    public void ConditionalExpressionFalseCase()
    {
        var sentence = "let a value equals 10. if a value is 11 then multiply a value by 42.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(10, (long)variableValue);
    }
    [Fact]
    public void LogicalExpressionNotEqual()
    {
        var sentence = "let a value equals 10. if a value is not 11 then multiply a value by 42.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(420, (long)variableValue);
    }
    [Fact]
    public void LogicalExpressionLess()
    {
        var sentence = "let a value equals 10. if a value less than 11 then multiply a value by 42.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(420, (long)variableValue);
    }
    [Fact]
    public void LogicalExpressionGreater()
    {
        var sentence = "let a value equals 10. if a value greater than 10 then multiply a value by 42.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(10, (long)variableValue);
    }
    [Fact]
    public void LogicalExpressionLessThan()
    {
        var sentence = "let a value equals 10. if a value at most 10 then multiply a value by 6. if a value at most 9 then multiply a value by 7.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(60, (long)variableValue);
    }
    [Fact]
    public void LogicalExpressionGreaterOrEqual()
    {
        var sentence = "let a value equals 10. if a value at least 10 then multiply a value by 6. if a value at least 9 then multiply a value by 7.\n";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("value");
        Assert.Equal(420, (long)variableValue);
    }

    [Fact]
    public void DeclareFunction()
    {
        var sentence = @"To Calculate factorial of a number: if a number is 0 then result is 1.
if a number is 1 then result is 1.
let a previous number is a number minus 1.
calculate factorial of a previous number into a previous factorial.
result is a previous factorial multiply a number.

";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var factorialFunction = vm.GetFunctionDeclaration("Calculate factorial of@1");
        Assert.NotNull(factorialFunction);
    }

    [Fact]
    public void InvokeFunction()
    {
        var sentence = @"To Calculate factorial of a number: if a number is 0 then result is 1.
if a number is 1 then result is 1.
let a previous number is a number minus 1.
calculate factorial of a previous number into a previous factorial.
result is a previous factorial multiply a number.

Calculate factorial of 5 into a factorial.
";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        var variableValue = vm.GetVariableValue("factorial");
        Assert.Equal(120, (long)variableValue);
        var variableDeclaration = vm.GetVariableDeclaration("factorial");
        Assert.NotNull(variableDeclaration);
    }

    [Fact]
    public void ShapeDeclaration()
    {
        var sentence = @"a button is a thing with a roundy box and a name.
";

        var vm = new EngLangVm();
        vm.ExecuteCode(sentence);

        //var variableDeclaration = vm.GetVariableDeclaration("button");
        //Assert.NotNull(variableDeclaration);
        var typeDeclaration = vm.GetTypeDeclaration("button");
        Assert.NotNull(typeDeclaration);
    }
}
