using System.Text.RegularExpressions;
using ClefViewer.Core.Render;
using Destructurama;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Formatting.Compact.Reader;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Serilog.Templates;

namespace ClefViewer.Core.Models;

public class Clef : ICanUnwrap
{
    private const string OutputTemplate = "{#if SourceContext is not null}" +
                                          "[{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] " +
                                          "{#end}" +
                                          "{@m:l}";

    private static readonly ExpressionTemplate ExpressionTemplate = new(OutputTemplate);

    private readonly LogEvent logEvent;

    /// <summary>
    /// @t Timestamp in ISO8601
    /// </summary>
    public DateTimeOffset Timestamp => logEvent.Timestamp;

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

    public IReadOnlyDictionary<string, object> Properties { get; private set; }

    /// <summary>
    /// @x Exception
    /// </summary>
    public Exception? Exception => logEvent.Exception;

    public Clef(LogEvent ev)
    {
        logEvent = ev;
        Properties =
            new Dictionary<string, object>(ev.Properties.Select(p => new KeyValuePair<string, object>(p.Key, p.Value)));
    }

    public Clef(string message, LogEventLevel level, DateTimeOffset offset)
    {
        Message = message;
        logEvent = new LogEvent(offset, level, null, new MessageTemplate(new List<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>());
        Properties = new Dictionary<string, object>();
    }

    public string Render()
    {
        if (Message is null)
        {
            var writer = new StringWriter();
            ExpressionTemplate.Format(logEvent, writer);

            Message = writer.ToString().TrimEnd('\n').TrimEnd('\r');
        }

        return Message;
    }

    public static Clef Parse(string input)
    {
        try
        {
            var ev = LogEventReader.ReadFromString(input);
            return new Clef(ev);
        }
        catch (Exception e)
        {
            return new Clef($"Error parsing log event: {e.Message}", LogEventLevel.Error, DateTimeOffset.Now);
        }
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
        if (Exception is not null)
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