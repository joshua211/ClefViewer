using Newtonsoft.Json;

namespace ClefViewer.Core.Tests;

public class JsonConverterFixture
{
    public void Init()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new ClefJsonConverter() }
        }; 
    }
}