using ClefViewer.Core.Models;

namespace ClefViewer.Core.Context;

public class Page
{
    public IReadOnlyCollection<Clef> Events { get; private set; }

    public int PageIndex { get; private set; }

    public bool HasNextPage { get; private set; }
    

    public Page(IReadOnlyCollection<Clef> events, int pageIndex, bool hasNextPage)
    {
        Events = events;
        PageIndex = pageIndex;
        HasNextPage = hasNextPage;
    }
}