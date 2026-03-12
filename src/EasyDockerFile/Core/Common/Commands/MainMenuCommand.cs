using EasyDockerFile.Core.Types.GitTypes;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace EasyDockerFile.Core.Common.Commands;
public class MainMenuCommand : AsyncCommand<MainMenuSettings>
{
    private static bool RepoLinkIsMissing([NotNull] MainMenuSettings settings) 
    {
        if (settings.RepoLink == null) {
            var eMessage = Markup.Escape("[ERROR]: A link to a git repository must be specified.");
            AnsiConsole.MarkupLine($"[red]{eMessage}[/]");
            Console.WriteLine("[INFO]: Use the --help flag for more information.");
            return true;
        }
        return false;
        
    }


    private static bool TokenOrPrivateFlagMissing([NotNull] MainMenuSettings settings) 
    {
        if (settings.PrivateFlagSet && settings.OAuthToken == null) 
        {
            var eMessage = Markup.Escape("[ERROR]: When using the --private flag, an OAuth Token must be specified.");
            AnsiConsole.MarkupLine($"[red]{eMessage}[/]");
            Console.WriteLine("[INFO]: Use the --help flag for more information.");
            return true;
        }

        if (!settings.PrivateFlagSet && settings.OAuthToken != null) 
        {
            var errorMessage = Markup.Escape("[ERROR]: Unexpected OAuth Token specified.");
            var warningMessage = Markup.Escape("[WARNING]: Please remove the token argument, or include --private in your command.");
            AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
            AnsiConsole.MarkupLine($"[yellow]{warningMessage}[/]");
            Console.WriteLine("[INFO]: Use the --help flag for more information.");
            return true;
        }
        return false;
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] MainMenuSettings settings, CancellationToken cancellationToken) 
    {
        if (RepoLinkIsMissing(settings)) {
            return 1;
        }

        if (TokenOrPrivateFlagMissing(settings)) {
            return 1;
        }
        
        AnsiConsole.MarkupLine($"[green]Attempting to build from:[/] {settings.RepoLink}");

        var repoInfoObj = new RepoInfo(settings);
        var client = await RepoClient.CreateAsync(repoInfoObj);

        await client.UpdateBranchesAsync();
        client.UpdateStatus();

        await client.UpdateFileNamesAsync();

        
        Console.WriteLine(client);
        return 0;
            
    }
}