using ClefViewer.Console.Controller;

namespace ClefViewer.Console.Display.Abstractions;

public interface IContextDisplay
{
    public Task StartRenderAsync(RenderProperties properties);
    public void Render(RenderProperties properties);

    public string? ShowQueryPrompt();

    public string? ShowSearchPrompt();

    public List<string> ShowLevelSelector();

    public Task WaitForReadyAsync();
}