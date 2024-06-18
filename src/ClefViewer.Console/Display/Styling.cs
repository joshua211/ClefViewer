namespace ClefViewer.Console.Display;

public static class Styling
{
    public static string AsError(string input)
    {
        return $"[red]{input}[/]";
    }

    public static string AsWarning(string input)
    {
        return $"[yellow]{input}[/]";
    }

    public static string AsInfo(string input)
    {
        return $"[green]{input}[/]";
    }

    public static string AsDebug(string input)
    {
        return $"[blue]{input}[/]";
    }

    public static string AsTrace(string input)
    {
        return $"[grey]{input}[/]";
    }

    public static string AsSearchText(string input)
    {
        return $"[italic aquamarine3]{input}[/]";
    }

    public static string AsQueryText(string input)
    {
        return $"[darkorange3]{input}[/]";
    }

    public static string AsHighlighted(string asStyled, string? textFilter)
    {
        if (string.IsNullOrEmpty(textFilter))
            return asStyled;
        return asStyled.Replace(textFilter, $"[on aquamarine3]{textFilter}[/]", StringComparison.OrdinalIgnoreCase);
    }
}