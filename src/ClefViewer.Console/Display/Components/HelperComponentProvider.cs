using ClefViewer.Console.Controller;
using ClefViewer.Console.Display.Components.Abstractions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components;

public class HelperComponentProvider : IHelperComponentProvider
{
    public IRenderable GetComponent(RenderProperties properties)
    {
        var grid = new Grid();
        grid.Expand();
        grid.Width(100);

        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();

        var firstTable = GetShortcutTable(new List<(string, string)>()
        {
            ("[deepskyblue3]<ctrl+f>[/]", "search"),
            ("[deepskyblue3]<ctrl+q>[/]", "query"),
            ("[deepskyblue3]<ctrl+e>[/]", "expand"),
        });

        var secondTable = GetShortcutTable(new List<(string, string)>()
        {
            ("[deepskyblue3]<shift+f>[/]", "remove filter"),
            ("[deepskyblue3]<shift+q>[/]", "remove query"),
            ("[deepskyblue3]<escape>[/]", "clear all filter"),
        });

        var thirdTable = GetShortcutTable(new List<(string, string)>()
        {
            ("[deepskyblue3]<ctrl+e>[/]", "expand line"),
            ("[deepskyblue3]<ctrl+b>[/]", "full screen"),
            ("[deepskyblue3]<ctrl+t>[/]", "toggle timestamp"),
        });

        grid.AddRow(firstTable, secondTable, thirdTable);

        return grid;
    }
    
    private Table GetShortcutTable(IEnumerable<(string, string)> shortcuts)
    {
        var table = new Table();
        table.AddColumn("Shortcut");
        table.AddColumn("Description");
        table.HideHeaders();
        table.Border(TableBorder.None);
        table.Expand();

        foreach (var (shortcut, description) in shortcuts)
        {
            table.AddRow(shortcut, description);
        }

        return table;
    }
}