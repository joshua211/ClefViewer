using Serilog.Core;
using Serilog.Events;

namespace ClefViewer.Core.Render;

public class StringLogSink : ILogEventSink
{
    public string? RenderedMessage { get; private set; }
    
    public void Emit(LogEvent logEvent)
    {
        var writer = new StringWriter();
        logEvent.RenderMessage(writer);
        
        RenderedMessage = writer.ToString();
    }
}