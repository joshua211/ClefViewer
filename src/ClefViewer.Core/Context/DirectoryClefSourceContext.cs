using ClefViewer.Core.Context.Abstractions;

namespace ClefViewer.Core.Context;

public class DirectoryClefSourceContext : IClefSourceContext
{
    private List<FileClefSourceContext> fileContexts;
    public string SourcePath { get; }
    public IReadOnlyDictionary<string, int>? EncounteredLevels { get; }

    public DirectoryClefSourceContext(string sourcePath, IReadOnlyDictionary<string, int>? encounteredLevels)
    {
        SourcePath = sourcePath;
        EncounteredLevels = encounteredLevels;

        fileContexts = new List<FileClefSourceContext>();
        foreach (var VARIABLE in Directory.GetFiles(sourcePath).Where(f => f.Split('.').Last() is var fileType && fileType is "txt" or "log"))
        {
            
        }

    }

    public Task<Page> GetPage(int page, string? textFilter, string? queryFilter, string[] levels)
    {
        throw new NotImplementedException();
    }


    public Task LoadLevelDataAsync()
    {
        throw new NotImplementedException();
    }
}