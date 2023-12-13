using ClefViewer.Console.Controller;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display.Components.Abstractions;

public interface IControlsComponentProvider
{
    IRenderable GetComponent(RenderProperties properties);
}