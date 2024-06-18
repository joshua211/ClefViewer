namespace ClefViewer.Core.Models;

public static class Levels
{
    public static IReadOnlyCollection<string> All { get; } = new List<string>
    {
        Error,
        Warning,
        Information,
        Debug,
        Verbose
    };

    public static string Error => "Error";
    public static string Warning => "Warning";
    public static string Information => "Information";
    public static string Debug => "Debug";
    public static string Verbose => "Verbose";
}