namespace EngLang.Vm;

public class VmValue
{
    public enum VmValueType
    {
        Long,
        Rational,
        String
    }
    public VmValueType Type { get; }
    public object Value { get; }
    public long? Divisor { get; }
    public string? Metric { get; }

    public VmValue(long rawValue)
    {
        this.Value = rawValue;
        this.Type = VmValueType.Long;
    }

    public VmValue(string rawValue)
    {
        this.Value = rawValue;
        this.Type = VmValueType.String;
    }
    public VmValue(long rawValue, long divisor)
    {
        this.Value = rawValue;
        this.Divisor = divisor;
        this.Type = VmValueType.Rational;
    }
    public VmValue(long rawValue, string metric)
    {
        this.Value = rawValue;
        this.Metric = metric;
        this.Type = VmValueType.Long;
    }
    public VmValue(long rawValue, long divisor, string metric)
    {
        this.Value = rawValue;
        this.Divisor = divisor;
        this.Metric = metric;
        this.Type = VmValueType.Rational;
    }

    public static VmValue operator +(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return new VmValue((long)a.Value + (long)b.Value);
        }
        throw new InvalidOperationException("Addition is only supported for Long types.");
    }

    public static VmValue operator -(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return new VmValue((long)a.Value - (long)b.Value);
        }
        throw new InvalidOperationException("Subtraction is only supported for Long types.");
    }

    public static VmValue operator *(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return new VmValue((long)a.Value * (long)b.Value);
        }
        throw new InvalidOperationException("Multiplication is only supported for Long types.");
    }

    public static VmValue operator /(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            if ((long)b.Value == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            return new VmValue((long)a.Value / (long)b.Value);
        }
        throw new InvalidOperationException("Division is only supported for Long types.");
    }

    public static bool operator >(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return (long)a.Value > (long)b.Value;
        }
        throw new InvalidOperationException("Comparison is only supported for Long types.");
    }
    public static bool operator <(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return (long)a.Value < (long)b.Value;
        }
        throw new InvalidOperationException("Comparison is only supported for Long types.");
    }
    public static bool operator >=(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return (long)a.Value >= (long)b.Value;
        }
        throw new InvalidOperationException("Comparison is only supported for Long types.");
    }
    public static bool operator <=(VmValue a, VmValue b)
    {
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return (long)a.Value <= (long)b.Value;
        }
        throw new InvalidOperationException("Comparison is only supported for Long types.");
    }

    public static bool operator ==(VmValue? a, VmValue? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return (long)a.Value == (long)b.Value;
        }
        throw new InvalidOperationException("Comparison is only supported for Long types.");
    }
    public static bool operator !=(VmValue? a, VmValue? b)
    {
        if (a is null && b is null) return false;
        if (a is null || b is null) return true;
        if (a.Type == VmValueType.Long && b.Type == VmValueType.Long)
        {
            return (long)a.Value != (long)b.Value;
        }
        throw new InvalidOperationException("Comparison is only supported for Long types.");
    }

    public static explicit operator long(VmValue b) => (long)b.Value;
    public static explicit operator string(VmValue b) => (string)b.Value;

}
