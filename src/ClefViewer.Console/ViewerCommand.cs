using ClefViewer.Console.CommandHandling;
using ClefViewer.Console.Controller;
using ClefViewer.Console.Display.Abstractions;
using ClefViewer.Console.Settings;
using ClefViewer.Core.Context;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ClefViewer.Console;

public class ViewerCommand : AsyncCommand<ViewerCommandSettings>
{
    private readonly IContextDisplay display;
    private readonly CommandHandler handler;

    public ViewerCommand(IContextDisplay display, CommandHandler handler)
    {
        this.display = display;
        this.handler = handler;
    }

    public override async Task<int> ExecuteAsync(CommandContext ctx, ViewerCommandSettings settings)
    {
        var path = settings.ContextPath ?? Directory.GetCurrentDirectory();
        var context = new FileClefSourceContext(path);
        var controller = new ContextController(context, display, handler);

        try
        {
            await controller.StartAsync();
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }

        AnsiConsole.Console.Input.ReadKey(true);

        return 1;
    }
}