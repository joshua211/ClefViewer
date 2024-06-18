using Spectre.Console;

namespace ClefViewer.Console.Display;

public class Aliases
{
    public static string LogLevelAlias(string level)
    {
        return level switch
        {
            "Critical" => "CRIT",
            "Error" => "ERR",
            "Warning" => "WAR",
            "Information" => "INF",
            "Informational" => "INF",
            "Debug" => "DBG",
            "Trace" => "TRC",
            "Verbose" => "VRB",
            _ => level
        };
    }

    public static Color LogLevelColor(string key)
    {
        return key switch
        {
            "Critical" => Color.Red,
            "Error" => Color.Red,
            "Warning" => Color.Yellow,
            "Information" => Color.Green,
            "Informational" => Color.Green,
            "Debug" => Color.Blue,
            "Trace" => Color.Grey,
            "Verbose" => Color.Grey,
            _ => Color.White
        };
    }
}