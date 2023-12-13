using System.Dynamic;
using ClefViewer.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClefViewer.Core;

public class ClefJsonConverter : JsonConverter<Clef>
{
    public override void WriteJson(JsonWriter writer, Clef? value, JsonSerializer serializer)
    {
        throw new Exception("This converter is only intended to be used for reading JSON.");
    }

    public override Clef? ReadJson(JsonReader reader, Type objectType, Clef? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        
        var timestamp = jObject["@t"]!.Value<DateTime>();
        jObject.TryGetValue("@m", out var message);
        jObject.TryGetValue("@mt", out var messageTemplate);
        jObject.TryGetValue("@l", out var level);
        jObject.TryGetValue("@x", out var exception);
        jObject.TryGetValue("@i", out var eventId);
        
        var properties = jObject.Properties()
            .Where(p => !p.Name.StartsWith("@"))
            .ToDictionary(p => p.Name, p => p.Value.Type switch
            {
                JTokenType.Integer =>  p.Value.ToObject<int>(),
                JTokenType.Float => p.Value.ToObject<float>(),
                JTokenType.String => p.Value.ToObject<string>(),
                JTokenType.Boolean => p.Value.ToObject<bool>(),
                JTokenType.Date => p.Value.ToObject<DateTime>(),
                JTokenType.Guid => p.Value.ToObject<Guid>(),
                JTokenType.Uri => p.Value.ToObject<Uri>(),
                JTokenType.Object => p.Value,
                _ => p.Value.ToObject<object>()
            });

        return new Clef(timestamp, message?.Value<string>(), messageTemplate?.Value<string>(), level?.Value<string>(),
            exception?.Value<string>() ?? string.Empty, eventId?.Value<string>() ?? string.Empty, properties);
    }
}