using ClefViewer.Core.Models;

namespace ClefViewer.Console.Controller;

public class RenderProperties
{
    public RenderProperties(IReadOnlyCollection<Clef> events, bool expandEvent, string textFilter,
        IReadOnlyCollection<string> levels, string queryFilter, string displayCommand, string contextSourceInfo,
        IReadOnlyDictionary<string, int>? encounteredLevels, int offset, bool fullscreen, bool showTimestamp)
    {
        Events = events;
        ExpandEvent = expandEvent;
        TextFilter = textFilter;
        Levels = levels;
        QueryFilter = queryFilter;
        DisplayCommand = displayCommand;
        ContextSourceInfo = contextSourceInfo;
        EncounteredLevels = encounteredLevels;
        Offset = offset;
        Fullscreen = fullscreen;
        ShowTimestamp = showTimestamp;
    }

    public IReadOnlyCollection<Clef> Events { get; private set; }
    public bool ExpandEvent { get; private set; }
    public string? TextFilter { get; private set; }
    public IReadOnlyCollection<string> Levels { get; private set; }
    public string? QueryFilter { get; private set; }
    public string DisplayCommand { get; private set; }
    public string ContextSourceInfo { get; private set; }
    public IReadOnlyDictionary<string, int>? EncounteredLevels { get; private set; }
    public int Offset { get; private set; }
    public bool Fullscreen { get; private set; }
    public bool ShowTimestamp { get; private set; }
}