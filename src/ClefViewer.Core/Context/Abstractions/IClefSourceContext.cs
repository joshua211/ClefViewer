namespace ClefViewer.Core.Context.Abstractions;

public interface IClefSourceContext
{
    string SourcePath { get; }

    IReadOnlyDictionary<string, int>? EncounteredLevels { get; }
    Task<Page> GetPage(int page, string? textFilter, string? queryFilter, string[] levels);

    Task LoadLevelDataAsync();
}