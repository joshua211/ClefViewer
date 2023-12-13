using ClefViewer.Console.Controller;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components.Abstractions;

public interface IContextComponentProvider
{
    IRenderable GetComponent(RenderProperties properties);
}