using Serilog.Events;

namespace ClefViewer.Core.Models;

public class PropertyFactory
{
    public static LogEventProperty CreateProperty(string name, object value)
    {
        return new LogEventProperty(name, CreatePropertyValue(name, value));
    }
    
    private static LogEventPropertyValue  CreatePropertyValue(object value)
    {
        // check if value is object and create StructureValue
        
        if(value is null)
            return new ScalarValue(null);
        
        if(value.GetType().IsClass)
            return new StructureValue(value.GetType().GetProperties().Select(p => CreateProperty(p.Name, p.GetValue(value))), null);
        // check if value is array and create SequenceValue
        
        if(value.GetType().IsArray)
            return new SequenceValue(value.GetType().GetProperties().Select(p => CreatePropertyValue(p.GetValue(value))));
    }
}