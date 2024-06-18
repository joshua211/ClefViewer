namespace ClefViewer.Core.Models;

public class WrappedPrimitive
{
    public WrappedPrimitive(object value)
    {
        Value = value;
    }

    public object Value { get; }

    public static implicit operator WrappedPrimitive(string value)
    {
        return new WrappedPrimitive(value);
    }

    public static implicit operator WrappedPrimitive(int value)
    {
        return new WrappedPrimitive(value);
    }

    public static implicit operator WrappedPrimitive(float value)
    {
        return new WrappedPrimitive(value);
    }

    public static implicit operator WrappedPrimitive(bool value)
    {
        return new WrappedPrimitive(value);
    }

    public static implicit operator WrappedPrimitive(DateTime value)
    {
        return new WrappedPrimitive(value);
    }

    public static implicit operator WrappedPrimitive(Guid value)
    {
        return new WrappedPrimitive(value);
    }

    public static implicit operator WrappedPrimitive(Uri value)
    {
        return new WrappedPrimitive(value);
    }

    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }
}