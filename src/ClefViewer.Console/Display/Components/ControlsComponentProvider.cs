using System.Text;
using ClefViewer.Console.Controller;
using ClefViewer.Console.Display.Components.Abstractions;
using ClefViewer.Core.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components;

public class ControlsComponentProvider : IControlsComponentProvider
{
    public IRenderable GetComponent(RenderProperties properties)
    {
        var table = new Table();
        table.HideHeaders();
        table.Border(TableBorder.None);
        table.AddColumn("LogLevel");
        table.AddColumn("TextFilter");
        table.AddColumn("Query");

        var sb = new StringBuilder();
        sb.AppendLine(properties!.Levels.Contains(Levels.Error) ? "[[X]] [red]Error[/]" : "[[ ]] [gray]Error[/]");
        sb.AppendLine(properties!.Levels.Contains(Levels.Warning)
            ? "[[X]] [yellow]Warning[/]"
            : "[[ ]] [gray]Warning[/]");
        sb.AppendLine(
            properties!.Levels.Contains(Levels.Information) ? "[[X]] [green]Info[/]" : "[[ ]] [gray]Info[/]");
        sb.AppendLine(properties!.Levels.Contains(Levels.Debug) ? "[[X]] [blue]Debug[/]" : "[[ ]] [gray]Debug[/]");
        sb.AppendLine(properties!.Levels.Contains(Levels.Verbose)
            ? "[[X]] [gray]Verbose[/]"
            : "[[ ]] [gray]Verbose[/]");

        var logLevelPanel = new Panel(sb.ToString());
        logLevelPanel.Header("Log Levels");

        var textFilterPanel = new Panel(Styling.AsSearchText(string.IsNullOrEmpty(properties.TextFilter) ? "None" : properties.TextFilter));
        textFilterPanel.Header("Text Filter");

        var queryPanel = new Panel(Styling.AsQueryText(string.IsNullOrEmpty(properties.QueryFilter) ? "None" : properties.QueryFilter));
        queryPanel.Header("Query");

        table.AddRow(logLevelPanel, textFilterPanel, queryPanel);

        return table;
    }
    
    
}