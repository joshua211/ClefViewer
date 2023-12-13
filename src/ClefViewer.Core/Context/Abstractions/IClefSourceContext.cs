using ClefViewer.Core.Models;

namespace ClefViewer.Core.Context.Abstractions;

public interface IClefSourceContext
{
    Task<Page> GetPage(int page, string? textFilter, string? queryFilter, string[] levels);

    string SourcePath { get; }
    
    IReadOnlyDictionary<string, int>? EncounteredLevels { get; }
    
    Task LoadLevelDataAsync();
}