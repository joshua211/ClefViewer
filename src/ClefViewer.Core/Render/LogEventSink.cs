using Serilog.Core;
using Serilog.Events;

namespace ClefViewer.Core.Render;

public class LogEventSink : ILogEventSink
{
    public LogEvent LogEvent { get; private set; }


    public void Emit(LogEvent logEvent)
    {
        LogEvent = logEvent;
    }
}