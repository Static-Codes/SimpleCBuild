using EasyDockerFile.Core.Common.Commands;
using Spectre.Console.Cli;
using static EasyDockerFile.Core.Helpers.DirectoryHelper;
using static Global.Logging;

var app = new CommandApp<MainMenuCommand>();

var cancellationTokenSource = new CancellationTokenSource();
  
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // Preventing the process from terminating immediately
    cancellationTokenSource.Cancel();
    WriteErrorMessage("Cancellation requested...", exitCode: 1, exit: true);
};

app.Configure(config =>
{
    config.SetApplicationName("SimpleCBuild");
});

// Creates directories for cache, config, data, and output
CreateAppDirectories();

// Entry point to the application.
return await app.RunAsync(args, cancellationTokenSource.Token);