using ClefViewer.Console.Controller;
using ClefViewer.Console.Display.Components.Abstractions;
using ClefViewer.Core.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components;

public class MessageComponentProvider : IMessageComponentProvider
{
    public IRenderable GetComponent(RenderProperties properties, int availableRows)
    {
        var events = properties?.Events ?? new List<Clef>();
        events = events.Skip(properties.Offset).Take(availableRows).ToList(); // Only take as much as we can render

        if (properties!.ExpandEvent)
        {
            var firstLine = events.FirstOrDefault();
            if (firstLine == null)
                return new Table();

            var asStyled = AsStyledMessage(firstLine);
            var root = new Tree(asStyled);

            foreach (var node in firstLine.Unwrap())
            {
                PopulateTree(root, node);
            }

            var padder = new Padder(root, new Padding(2, 2));

            // Rows to get some padding
            return padder;
        }

        var table = new Table();

        if(properties.ShowTimestamp)
            table.AddColumn("Time");
        table.AddColumn("Level");
        table.AddColumn("Message");

        foreach (var ev in events)
        {
            var asStyled = AsStyledMessage(ev);

            asStyled = Styling.AsHighlighted(asStyled, properties.TextFilter);
            var comp = new List<string>();
            if(properties.ShowTimestamp)
                comp.Add(ev.Timestamp.ToString("T"));
            comp.Add( Aliases.LogLevelAlias(ev.Level));
            comp.Add(asStyled);
            table.AddRow(comp.ToArray());
        }

        return table;
    }
    
    private static void PopulateTree(IHasTreeNodes tree, WrappedPrimitive wrapped)
    {
        if (wrapped is WrappedComplex complex)
        {
            var newNode = tree.AddNode(complex.Key.EscapeMarkup());
            foreach (var sub in complex.Unwrap())
            {
                PopulateTree(newNode, sub);
            }
        }
        else
        {
            tree.AddNode(wrapped.ToString().EscapeMarkup());
        }
    }
    
    private static string AsStyledMessage(Clef ev)
    {
        var renderedMessage = ev.Render().EscapeMarkup();
        var asStyled = ev.Level switch
        {
            "Error" => Styling.AsError(renderedMessage),
            "Warning" => Styling.AsWarning(renderedMessage),
            "Information" => Styling.AsInfo(renderedMessage),
            "Debug" => Styling.AsDebug(renderedMessage),
            "Verbose" => Styling.AsTrace(renderedMessage),
        };

        return asStyled;
    }
}