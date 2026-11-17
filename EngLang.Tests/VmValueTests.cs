using EngLang.Vm;
using Xunit;

namespace EngLang.Tests;

public class VmValueTests
{
    [Fact]
    public void MathOperatoinsOverNatural()
    {
        var op1 = new VmValue(10);
        var op2 = new VmValue(2);

        var sum = op1 + op2;
        Assert.Equal(VmValue.VmValueType.Long, sum.Type);
        Assert.Equal(12L, sum.Value);

        var diff = op1 - op2;
        Assert.Equal(VmValue.VmValueType.Long, diff.Type);
        Assert.Equal(8L, diff.Value);

        var mul = op1 * op2;
        Assert.Equal(VmValue.VmValueType.Long, mul.Type);
        Assert.Equal(20L, mul.Value);

        var div = op1 / op2;
        Assert.Equal(VmValue.VmValueType.Long, div.Type);
        Assert.Equal(5L, div.Value);
    }

    [Fact]
    public void ComparisonOperatorsOverNatural()
    {
        var op1 = new VmValue(10);
        var op2 = new VmValue(2);
        Assert.True(op1 > op2);
        Assert.False(op1 < op2);
        Assert.True(op1 >= op2);
        Assert.False(op1 <= op2);
        Assert.False(op1 == op2);
        Assert.True(op1 != op2);
    }
}
