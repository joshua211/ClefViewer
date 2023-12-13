using System.Text;
using ClefViewer.Console.Controller;
using ClefViewer.Console.Display.Abstractions;
using ClefViewer.Console.Display.Components.Abstractions;
using ClefViewer.Core.Context;
using ClefViewer.Core.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ClefViewer.Console.Display;

public class ConsoleContextDisplay : IContextDisplay
{
    private readonly IContextComponentProvider contextComponentProvider;
    private readonly IControlsComponentProvider controlsComponentProvider;
    private readonly IMessageComponentProvider messageComponentProvider;
    private readonly IHelperComponentProvider helperComponentProvider;

    private bool shouldRender = false;
    private bool isReady = true;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
    private RenderProperties? properties;

    public ConsoleContextDisplay(IContextComponentProvider contextComponentProvider,
        IControlsComponentProvider controlsComponentProvider, IMessageComponentProvider messageComponentProvider,
        IHelperComponentProvider helperComponentProvider)
    {
        this.contextComponentProvider = contextComponentProvider;
        this.controlsComponentProvider = controlsComponentProvider;
        this.messageComponentProvider = messageComponentProvider;
        this.helperComponentProvider = helperComponentProvider;
    }

    public async Task StartRenderAsync(RenderProperties props)
    {
        this.properties = props;
        shouldRender = true;
        await RenderContent();
    }

    public void Render(RenderProperties props)
    {
        this.properties = props;
        shouldRender = true;
    }

    public string? ShowQueryPrompt()
    {
        PrepareNewScreen();
        var value = AnsiConsole.Ask<string?>("Query:", null);
        PrepareMainScreen();

        return value;
    }

    public string? ShowSearchPrompt()
    {
        PrepareNewScreen();
        var value = AnsiConsole.Ask<string?>("Search:", null);
        PrepareMainScreen();

        return value;
    }

    public List<string> ShowLevelSelector()
    {
        PrepareNewScreen();
        var value = AnsiConsole.Prompt(new MultiSelectionPrompt<string>().Title("Select levels")
            .PageSize(10)
            .AddChoices(Levels.All));

        PrepareMainScreen();

        return value;
    }

    public Task WaitForReadyAsync()
    {
        return taskCompletionSource.Task;
    }

    private void PrepareNewScreen()
    {
        cancellationTokenSource.Cancel();
        WaitForConsole();
        AnsiConsole.Clear();
        isReady = false;
    }

    private void PrepareMainScreen()
    {
        isReady = true;
        cancellationTokenSource = new CancellationTokenSource();
        taskCompletionSource.SetResult();
        taskCompletionSource = new TaskCompletionSource();
    }

    private void WaitForConsole()
    {
        while (!isReady)
        {
            Thread.Sleep(100);
        }
    }

    private async Task RenderContent()
    {
        var rootLayout = new Layout();
        var contextLayout = new Layout("context", contextComponentProvider.GetComponent(properties!));
        var messageLayout = new Layout("message", messageComponentProvider.GetComponent(properties!, AvailableRows));
        var footer = new Layout("footer");
        var footerLayoutLeft = new Layout("footerLeft", controlsComponentProvider.GetComponent(properties!));
        var footerLayoutRight = new Layout("footerRight", helperComponentProvider.GetComponent(properties!));
        footerLayoutLeft.Ratio(3);
        footerLayoutRight.Ratio(6);

        rootLayout.SplitRows(
            contextLayout,
            messageLayout,
            footer.SplitColumns(footerLayoutLeft, footerLayoutRight)
        );

        rootLayout["context"].Size(3);
        rootLayout["message"].MinimumSize(AvailableRows);
        rootLayout["footer"].Size(8);

        await AnsiConsole.Live(rootLayout).StartAsync(async ctx =>
        {
            isReady = false;
            var token = cancellationTokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(50, token);
                    if (shouldRender)
                    {
                        if (properties!.Fullscreen)
                        {
                            rootLayout["context"].Invisible();
                            rootLayout["footer"].Invisible();
                        }
                        else
                        {
                            rootLayout["context"].Visible();
                            rootLayout["footer"].Visible();
                            rootLayout["context"].Update(contextComponentProvider.GetComponent(properties!));
                            rootLayout["footer"]["footerLeft"]
                                .Update(controlsComponentProvider.GetComponent(properties!));
                        }

                        rootLayout["message"].Update(messageComponentProvider.GetComponent(properties!, AvailableRows));
                        ctx.Refresh();
                        shouldRender = false;
                    }
                }
                catch (TaskCanceledException)
                {
                }
            }
        });
        isReady = true;
    }

    private int AvailableRows => AnsiConsole.Console.Profile.Height - (properties!.Fullscreen ? 2 :17); // 17 is control + info space
}