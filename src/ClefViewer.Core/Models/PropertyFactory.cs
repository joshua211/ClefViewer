using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace ClefViewer.Core.Models;

public static class PropertyFactory
{
    private const string TypeTagPropertyName = "$type";
    private const string InvalidPropertyNameSubstitute = "(unnamed)";

    public static LogEventProperty CreateProperty(string name, object value)
    {
        return new LogEventProperty(name, CreatePropertyValue(value));
    }

    private static LogEventPropertyValue CreatePropertyValue(object? value)
    {
        // check if value is object and create StructureValue

        if (value is null)
            return new ScalarValue(null);

        if (value is JObject obj)
        {
            obj.TryGetValue(TypeTagPropertyName, out var tt);
            return new StructureValue(
                obj.Properties().Where(kvp => kvp.Name != TypeTagPropertyName)
                    .Select(kvp => CreateProperty(kvp.Name, kvp.Value)),
                tt?.Value<string>());
        }

        if (value.GetType() is { IsClass: true } t && t != typeof(string))
            return new StructureValue(value.GetType().GetProperties()
                .Select(p => CreateProperty(p.Name, p.GetValue(value))));
        // check if value is array and create SequenceValue

        if (value.GetType().IsArray)
            return new SequenceValue(
                value.GetType().GetProperties().Select(p => CreatePropertyValue(p.GetValue(value))));

        return new ScalarValue(value);
    }
}