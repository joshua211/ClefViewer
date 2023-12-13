using Newtonsoft.Json.Linq;

namespace ClefViewer.Core.Models;

public class WrappedComplex : WrappedPrimitive, ICanUnwrap
{
    public string Key { get; private set; }
    
    public WrappedComplex(string key, JObject value) : base(value)
    {
        Key = key;
    }

    public IReadOnlyCollection<WrappedPrimitive> Unwrap()
    {
        var unwrapped = new List<WrappedPrimitive>();
        var value = (JObject) Value;

        foreach (var property in value.Properties())
        {
            if (property.Value is JObject)
            {
                unwrapped.Add(new WrappedComplex(property.Name, (JObject) property.Value));
            }
            else
            {
                unwrapped.Add(new WrappedPrimitive($"{property.Name}: {property.Value}"));
            }
        }

        return unwrapped;
    }
}