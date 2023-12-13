using ClefViewer.Core.Context.Abstractions;
using ClefViewer.Core.Models;
using Serilog.Expressions;

namespace ClefViewer.Core.Context;

public class FileClefSourceContext : IClefSourceContext
{
    private const int MaxPageSize = 1000;

    private readonly string sourcePath;
    private readonly Dictionary<string, int> encounteredLevels;
    private Page? lastFrame;
    private bool hasLoadedLevels = false;

    public FileClefSourceContext(string sourcePath)
    {
        this.sourcePath = sourcePath;
        encounteredLevels = new Dictionary<string, int>();
    }

    public async Task LoadLevelDataAsync()
    {
        await using var file = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(file);
        var lines = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
                continue;

            lines.Add(line);
        }

        foreach (var line in lines)
        {
            var @event = Clef.Parse(line);
            if (encounteredLevels.ContainsKey(@event.Level))
            {
                encounteredLevels[@event.Level]++;
            }
            else
            {
                encounteredLevels.Add(@event.Level, 1);
            }
        }

        hasLoadedLevels = true;
    }

    public async Task<Page> GetPage(int pageNumber, string? textFilter, string? queryFilter, string[] levels)
    {
        var reverseReader = new ReverseLineReader(sourcePath);
        var events = new List<Clef>();

        var offset = pageNumber * lastFrame?.Events.Count ?? pageNumber * MaxPageSize;
        var currentOffset = 0;
        using var enumerator = reverseReader.GetEnumerator();

        while (enumerator.MoveNext())
        {
            {
                var line = enumerator.Current;
                if (currentOffset < offset)
                {
                    currentOffset++;
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                    throw new Exception();

                if (textFilter is not null &&
                    !line.Contains(textFilter,
                        StringComparison.OrdinalIgnoreCase)) // only take lines that fulfill the text filter
                    continue;

                var @event = Clef.Parse(line);

                if (!levels.Contains(@event.Level)) // only take lines that fulfill the level filter
                    continue;

                CompiledExpression queryExpression = null!;
                if (queryFilter is not null &&
                    SerilogExpression.TryCompile(queryFilter, out queryExpression, out var error)) // TODO handle error
                {
                    if (!@event.Matches(queryExpression)) // only take lines that fulfill the query filter
                        continue;
                }

                events.Add(@event);
                if (events.Count >= MaxPageSize)
                    break;
            }
        }
         

        lastFrame = new Page(events, pageNumber, enumerator.MoveNext());

        return lastFrame;
    }

    public string SourcePath => sourcePath;
    public IReadOnlyDictionary<string, int>? EncounteredLevels => hasLoadedLevels ? encounteredLevels : null;
}