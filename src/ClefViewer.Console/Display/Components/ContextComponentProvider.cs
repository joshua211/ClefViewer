using ClefViewer.Console.Controller;
using ClefViewer.Console.Display.Components.Abstractions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components;

public class ContextComponentProvider : IContextComponentProvider
{
    public IRenderable GetComponent(RenderProperties properties)
    {
        var table = new Table();
        table.Border(TableBorder.None);
        table.HideHeaders();
        table.AddColumn("Command", column => column.Width(10));
        table.AddColumn("File Context", column => { column.Width(30); });
        table.AddColumn("Breakdown");

        var action = new Align(new Markup($"[gray]{properties!.DisplayCommand}[/]"), HorizontalAlignment.Center,
            VerticalAlignment.Middle);

        var path = new Panel(
            new TextPath(properties.ContextSourceInfo));
        path.Header("Context");

        IRenderable breakdownSlot;
        if (properties.EncounteredLevels != null)
        {
            var breakdown = new BreakdownChart();

            foreach (var encountered in properties.EncounteredLevels)
            {
                breakdown.AddItem(encountered.Key, encountered.Value, Aliases.LogLevelColor(encountered.Key));
            }

            breakdownSlot = breakdown;
        }
        else
        {
            breakdownSlot = new Align(new Markup("[gray]Loading Data...[/]"), HorizontalAlignment.Center,
                VerticalAlignment.Middle);
        }


        table.AddRow(action, path, breakdownSlot);

        return table;
    }
}