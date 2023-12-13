using ClefViewer.Console.Controller;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components.Abstractions;

public interface IMessageComponentProvider
{
    IRenderable GetComponent(RenderProperties properties, int availableRows);
}