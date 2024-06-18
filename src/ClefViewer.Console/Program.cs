using ClefViewer.Console;
using ClefViewer.Console.CommandHandling;
using ClefViewer.Console.Controller;
using ClefViewer.Console.DI;
using ClefViewer.Console.Display;
using ClefViewer.Console.Display.Abstractions;
using ClefViewer.Console.Display.Components;
using ClefViewer.Console.Display.Components.Abstractions;
using ClefViewer.Core.Context;
using ClefViewer.Core.Context.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services.AddSingleton<IContextDisplay, ConsoleContextDisplay>();
services.AddSingleton<IClefSourceContext, FileClefSourceContext>();
services.AddSingleton<ContextController>();
services.AddSingleton<CommandHandler>();
services.AddTransient<IContextComponentProvider, ContextComponentProvider>();
services.AddTransient<IControlsComponentProvider, ControlsComponentProvider>();
services.AddTransient<IMessageComponentProvider, MessageComponentProvider>();
services.AddTransient<IHelperComponentProvider, HelperComponentProvider>();

var registar = new TypeRegistrar(services);

var app = new CommandApp<ViewerCommand>(registar);
app.Configure(c =>
{
#if DEBUG
    c.PropagateExceptions();
    c.ValidateExamples();
#endif
    c.SetApplicationName("ClefViewer");
});
return await app.RunAsync(args);