using ClefViewer.Console.CommandHandling;
using ClefViewer.Console.Display.Abstractions;
using ClefViewer.Core;
using ClefViewer.Core.Context;
using ClefViewer.Core.Context.Abstractions;
using ClefViewer.Core.Models;
using Newtonsoft.Json;

namespace ClefViewer.Console.Controller;

public class ContextController
{
    private readonly IClefSourceContext context;
    private readonly IContextDisplay display;
    private readonly CommandHandler commandHandler;

    private Page previousPage = null!;
    private Page currentPage = null!;
    private Page nextPage = null!;
    private int offset = 0;
    private bool expandFirstLine = false;

    private string? textFilter = null;
    private string? queryFilter = null;
    private List<string> levels;
    private bool fullscreen = false;
    private bool showTimestamp = true;

    public ContextController(IClefSourceContext context, IContextDisplay display, CommandHandler handler)
    {
        this.context = context;
        this.display = display;
        commandHandler = handler;
        commandHandler.CommandReceived += HandleCommand;
        levels = Levels.All.ToList();
    }

    public async Task StartAsync()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new ClefJsonConverter() }
        }; 
        
        await LoadPages();
        commandHandler.StartCapture();
        _ = Task.Run(async () =>
        {
            await context.LoadLevelDataAsync();
            HandleCommand(this, Command.RefreshUi);
        });

        while (true)
        {
            await display.StartRenderAsync(CollectProperties());
            await display.WaitForReadyAsync();
        }
    }

    private RenderProperties CollectProperties()
    {
        return new RenderProperties(
            currentPage.Events,
            expandFirstLine,
            textFilter ?? "",
            levels,
            queryFilter ?? "",
            commandHandler.Latest.ToString(),
            context.SourcePath,
            context.EncounteredLevels,
            offset,
            fullscreen,
            showTimestamp
        );
    }

    private async Task LoadPages()
    {
        offset = 0;
        currentPage = await context.GetPage(0, textFilter, queryFilter, levels.ToArray());
        if (currentPage.HasNextPage)
            nextPage = await context.GetPage(1, textFilter, queryFilter, levels.ToArray());
        offset = 0;
    }

    private async Task LoadNextPage()
    {
        previousPage = currentPage;
        currentPage = nextPage;
        offset = 0;
        if (currentPage.HasNextPage)
            nextPage = await context.GetPage(currentPage.PageIndex + 1, textFilter, queryFilter, levels.ToArray());
    }

    private async Task LoadPreviousPage()
    {
        nextPage = currentPage;
        currentPage = previousPage;
        offset = currentPage.Events.Count - 1;
        if (currentPage.PageIndex > 0)
            previousPage = await context.GetPage(currentPage.PageIndex - 1, textFilter, queryFilter, levels.ToArray());
    }

    private void HandleCommand(object? sender, Command e)
    {
        switch (e)
        {
            case Command.Down:
                if (offset >= currentPage.Events.Count - 1 &&
                    currentPage.HasNextPage)
                {
                    LoadNextPage().Wait();
                }
                else if (offset < currentPage.Events.Count - 1)
                {
                    offset++;
                }

                break;
            case Command.Up:
                if (offset > 0)
                {
                    offset--;
                }
                else if (currentPage.PageIndex > 0)
                {
                    LoadPreviousPage().Wait();
                }

                break;
            case Command.Query:
                commandHandler.StopCapture();
                queryFilter = display.ShowQueryPrompt();
                LoadPages().Wait();
                commandHandler.ResumeCapture();
                break;
            case Command.Search:
                commandHandler.StopCapture();
                textFilter = display.ShowSearchPrompt();
                LoadPages().Wait();
                commandHandler.ResumeCapture();
                break;
            case Command.ClearSearch:
                textFilter = null;
                LoadPages().Wait();
                break;
            case Command.ClearQuery:
                queryFilter = null;
                LoadPages().Wait();
                break;
            case Command.ClearAll:
                textFilter = null;
                queryFilter = null;
                LoadPages().Wait();
                break;
            case Command.SelectLevel:
                commandHandler.StopCapture();
                levels = display.ShowLevelSelector();
                LoadPages().Wait();
                commandHandler.ResumeCapture();
                break;
            case Command.ToggleExpand:
                expandFirstLine = !expandFirstLine;
                break;
            case Command.ToggleFullScreen:
                fullscreen = !fullscreen;
                break;
            case Command.ToggleTimestamp:
                showTimestamp = !showTimestamp;
                break;
        }

        display.Render(CollectProperties());
    }
}