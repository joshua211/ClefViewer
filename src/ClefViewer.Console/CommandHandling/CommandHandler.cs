using Spectre.Console;

namespace ClefViewer.Console.CommandHandling;

public class CommandHandler
{
    private bool isStopped;

    public CommandHandler()
    {
        Latest = Command.Start;
    }

    public Command Latest { get; private set; }

    public void StartCapture()
    {
        Task.Run(() =>
        {
            while (true)
            {
                if (isStopped || !AnsiConsole.Console.Input.IsKeyAvailable())
                    continue;

                var input = AnsiConsole.Console.Input.ReadKey(true);

                var command = input.Value.Key switch
                {
                    ConsoleKey.DownArrow => Command.Down,
                    ConsoleKey.UpArrow => Command.Up,
                    ConsoleKey.Escape => Command.ClearAll,
                    ConsoleKey.F => input.Value.Modifiers == ConsoleModifiers.Control ? Command.Search :
                        input.Value.Modifiers == ConsoleModifiers.Shift ? Command.ClearSearch : Command.None,
                    ConsoleKey.Q => input.Value.Modifiers == ConsoleModifiers.Control ? Command.Query :
                        input.Value.Modifiers == ConsoleModifiers.Shift ? Command.ClearQuery : Command.None,
                    ConsoleKey.L => input.Value.Modifiers == ConsoleModifiers.Control
                        ? Command.SelectLevel
                        : Command.None,
                    ConsoleKey.E => input.Value.Modifiers == ConsoleModifiers.Control
                        ? Command.ToggleExpand
                        : Command.None,
                    ConsoleKey.B => input.Value.Modifiers == ConsoleModifiers.Control
                        ? Command.ToggleFullScreen
                        : Command.None,
                    ConsoleKey.T => input.Value.Modifiers == ConsoleModifiers.Control
                        ? Command.ToggleTimestamp
                        : Command.None,
                    _ => Command.None
                };

                Latest = command;
                CommandReceived?.Invoke(this, command);
            }
        });
    }

    public void StopCapture()
    {
        isStopped = true;
    }

    public void ResumeCapture()
    {
        Latest = Command.Start;
        isStopped = false;
    }

    public event EventHandler<Command> CommandReceived;

    public void ClearLatest()
    {
        Latest = Command.None;
    }
}