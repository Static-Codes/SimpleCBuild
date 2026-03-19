using EasyDockerFile.Core.Common.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<MainMenuCommand>();

var cancellationTokenSource = new CancellationTokenSource();
  
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // Preventing the process from terminating immediately
    cancellationTokenSource.Cancel();
    AnsiConsole.WriteLine("[red]Cancellation requested...[/]");
    Environment.Exit(1);
};

app.Configure(config =>
{
    config.SetApplicationName("SimpleCBuild");
});

return await app.RunAsync(args, cancellationTokenSource.Token);