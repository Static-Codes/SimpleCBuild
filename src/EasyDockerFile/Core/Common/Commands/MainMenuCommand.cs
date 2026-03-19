using DockerFileSharp.Common;
using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Types.Git;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using static EasyDockerFile.Core.Helpers.InputHelper;
using static EasyDockerFile.Core.Loaders.FamilyLoader;

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

        var families = GetFamilies();


        // Choosing image family
        var familyNames = families.Select(fam => fam.Name);
        var familyChoice = AskForInput(
            message: "Please select your desired image family.", 
            options: MakeInputMenu(familyNames)
        );
        UserExitStatusCheck(familyChoice);

        var family = families.GetFamily(familyChoice);
        CheckForNullInput(family);


        // Choosing image version
        var imageNames = family.Images.Select(a => a.FullName);
        CheckForNullInput(imageNames);

        var imageChoice = AskForInput(
            message: "Please select your desired image version.", 
            options: MakeInputMenu(imageNames!)
        );
        UserExitStatusCheck(imageChoice);

        var selectedImage = family.Images.GetImage(imageChoice);
        CheckForNullInput(selectedImage);

        // Creating a DockerImage instance with the selected Image object
        var selectedDockerImage = new DockerImage(selectedImage);

        var buildInstructions = selectedDockerImage.GetInstructions(settings.RepoLink, repoInfoObj.RepoUrlObj.RepoName);
        var fileContents = buildInstructions.Build();
        return 0;
            
    }
}