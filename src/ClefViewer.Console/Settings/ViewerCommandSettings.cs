using System.ComponentModel;
using Spectre.Console.Cli;

namespace ClefViewer.Console.Settings;

public class ViewerCommandSettings : CommandSettings
{
    [Description("Path to search. Defaults to current directory.")]
    [CommandArgument(0, "[context]")]
    public string? ContextPath { get; init; }
}