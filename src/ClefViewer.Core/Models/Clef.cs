using System.Text.RegularExpressions;
using ClefViewer.Core.Render;
using Destructurama;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Parsing;

namespace ClefViewer.Core.Models;

public class Clef : ICanUnwrap
{
    private readonly LogEvent logEvent;

    /// <summary>
    /// @t Timestamp in ISO8601
    /// </summary>
    public DateTimeOffset Timestamp { get; private set; }

    /// <summary>
    /// @m Fully rendered Message
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// @mt Message template
    /// </summary>
    public string? MessageTemplate => logEvent.MessageTemplate.Text;

    /// <summary>
    /// @l Level of the event, absence of this field indicates level "Informational"
    /// </summary>
    public string Level => logEvent.Level.ToString();

    /// <summary>
    /// @x Exception
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Unique EventId
    /// </summary>
    public string EventId { get; private set; }

    public Dictionary<string, object> Properties { get; private set; }

    public Clef(DateTimeOffset timestamp, string? message, string? messageTemplate, string? level,
        string? exception,
        string? eventId, Dictionary<string, object> properties)
    {
        if (message is null && messageTemplate is null)
            throw new ArgumentException("Message and MessageTemplate cannot both be null.");

        Message = message;
        Timestamp = timestamp;
        Properties = properties;
        Exception = exception;
        EventId = string.IsNullOrEmpty(eventId) ? Guid.NewGuid().ToString() : eventId;

        var requiredProperties = GetRequiredProperties(messageTemplate!, properties);
        var parser = new MessageTemplateParser();
        var templ = parser.Parse(messageTemplate!);
        
        var properties = PropertyFa
        
        var ev = new LogEvent(Timestamp, LogEventLevel.Information, null, templ,
            requiredProperties);
        //TODO check this
        Enum.TryParse<LogEventLevel>(level ?? "Information", out var serilogLevel);

        // TODO create logEvent without writing to sink
        var sink = new LogEventSink();
        var logger = new LoggerConfiguration().MinimumLevel.Verbose()
            .Destructure.JsonNetTypes()
            .WriteTo.Sink(sink)
            .CreateLogger();

        logger.Write(serilogLevel, messageTemplate, requiredProperties);
        logEvent = sink.LogEvent;

        foreach (var prop in properties)
        {
            logEvent.AddPropertyIfAbsent(new LogEventProperty(prop.Key, new ScalarValue(prop.Value)));
        }
    }

    private object[] GetRequiredProperties(string messageTemplate, Dictionary<string, object> properties)
    {
        var regex = new Regex(@"(?<=\{)[^}]*(?=\})");
        var matches = regex.Matches(messageTemplate);

        // Split by : to avoid format specifications in name
        var requiredProperties = matches.Select(m =>
        {
            var sanitized = m.Value.Split(":").First()
                .Replace("@", string.Empty);
            return properties[sanitized];
        }).ToArray();

        return requiredProperties.ToArray();
    }

    public string Render()
    {
        if (Message is not null)
            return Message;
        
        var sink = new StringLogSink();
        var logger = new LoggerConfiguration()
            .Destructure.JsonNetTypes()
            .WriteTo.Sink(sink, LogEventLevel.Verbose)
            .MinimumLevel.Verbose()
            .CreateLogger();

        var requiredProperties = GetRequiredProperties(MessageTemplate!, Properties).ToArray();

        // TODO include Exception
        var exception = Exception is null ? null : new TextException(Exception);
        logger.Write(logEvent.Level, exception, MessageTemplate!, requiredProperties);
        
        var renderedMessage = sink.RenderedMessage ?? string.Empty;
        
        Message = renderedMessage;

        return renderedMessage;
    }

    public static Clef Parse(string input)
    {
        return JsonConvert.DeserializeObject<Clef>(input)!;
    }

    public bool Matches(CompiledExpression expression)
    {
        var x = expression(logEvent);
        if (x is ScalarValue { Value: bool and true })
            return true;
        return false;
    }

    public override string ToString()
    {
        return Render();
    }

    public IReadOnlyCollection<WrappedPrimitive> Unwrap()
    {
        var unwrapped = new List<WrappedPrimitive>();
        unwrapped.Add($"Timestamp: {Timestamp}");
        unwrapped.Add($"Level:  {Level}");
        if(!string.IsNullOrEmpty(Exception))
            unwrapped.Add($"Exception: {Exception}");
        
        foreach (var prop in Properties)
        {
            if (prop.Value is JObject obj)
            {
                var wrapped = new WrappedComplex(prop.Key, obj);
                unwrapped.Add(wrapped);
            }
            else
            {
                unwrapped.Add($"{prop.Key}: {prop.Value}");
            }
        }

        return unwrapped;
    }
}