namespace ClefViewer.Core.Models;

public class WrappedPrimitive
{
    public object Value { get; private set; }

    public WrappedPrimitive(object value)
    {
        Value = value;
    }
    
    public static implicit operator WrappedPrimitive(string value) => new(value);
    
    public static implicit operator WrappedPrimitive(int value) => new(value);
    
    public static implicit operator WrappedPrimitive(float value) => new(value);
    
    public static implicit operator WrappedPrimitive(bool value) => new(value);
    
    public static implicit operator WrappedPrimitive(DateTime value) => new(value);
    
    public static implicit operator WrappedPrimitive(Guid value) => new(value);
    
    public static implicit operator WrappedPrimitive(Uri value) => new(value);

    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }
}