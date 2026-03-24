using DockerFileSharp.Common;
using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Loaders;
using Global.Build;
using EasyDockerFile.Core.Types.Git;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static EasyDockerFile.Core.Helpers.InputHelper;
using static EasyDockerFile.Core.Loaders.FamilyLoader;
using static Global.Constants;
using static Global.Logging;

namespace EasyDockerFile.Core.Common.Commands;

public class MainMenuCommand : AsyncCommand<MainMenuSettings>
{
    private static async Task<bool> BuildContainer(string dockerfilePath, string systemName, string tagName)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"build -t {tagName} -f {dockerfilePath} .",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string output = await outputTask;
        string error = await errorTask;

        if (process.ExitCode != 0)
        {
            WriteWarningMessage($"Docker build failed for {systemName} with exit code {process.ExitCode}.");
            if (string.IsNullOrEmpty(error)) {
                return false;
            }

            if (error.Contains("ERROR: permission denied while trying to connect to the Docker daemon socket at")) {
                WriteErrorMessage("Unable to connect to the docker daemon, please ensure docker is running.", exitCode: 1, exit: true);
            }

            WriteErrorMessage(error);


            return false;
        }

        return true;
    }

    private static async Task<bool> CopyBuildArtifacts(string fullContainerPath, string hostOutputDirectory) 
    {
        var copyPSI = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"cp simplecbuild-temp:{fullContainerPath} {hostOutputDirectory}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var copyProcess = Process.Start(copyPSI);

        if (copyProcess == null) {
            WriteWarningMessage("Unable to copy build artifacts from the temporary docker container.");
            WriteErrorMessage("Variable 'copyProcess' is null", exitCode: 1, exit: true);
        }
        
        await copyProcess.WaitForExitAsync();

        bool copySuccess = copyProcess.ExitCode == 0;

        if (!copySuccess) {
            WriteErrorMessage(await copyProcess.StandardError.ReadToEndAsync());
        }

        return copySuccess;
    }
    private static async Task<(RepoInfo info, RepoClient client)> CreateRepoInfoAndClient([NotNull] MainMenuSettings settings) 
    {
        var repoInfoObj = new RepoInfo(settings);
        var client = await RepoClient.CreateAsync(repoInfoObj);

        await client.UpdateBranchesAsync();
        client.UpdateStatus();

        await client.UpdateBranchFileCount();

        await client.UpdateProjectLanguagesAsync();

        return (repoInfoObj, client);
    }
    private static async Task DeleteContainer() 
    {
        var removePSI = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "rm simplecbuild-temp",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var removeProcess = Process.Start(removePSI);

        await removeProcess!.WaitForExitAsync();

        if (removeProcess == null) {
            WriteWarningMessage("Unable to remove the temporary docker container.");
        }
    }    
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] MainMenuSettings settings, CancellationToken cancellationToken) 
    {
        if (RepoLinkIsMissing(settings)) {
            return 1;
        }

        if (TokenOrPrivateFlagMissing(settings)) {
            return 1;
        }
        
        // This is output as a single line, however, each part has its own color for added emphasis. 
        Console.Write("Attempting to build from: ");
        WriteStateMessage($"{settings.RepoLink}");

        (var repoInfoObj, var client) = await CreateRepoInfoAndClient(settings);
        
        // Testing client.ToString()
        Console.WriteLine(client);

        var selectedDockerImage = GetSelectedDockerImage();

        var buildSystemInfo = GetBuildSystemInfo(repoInfoObj);
        
        if (!buildSystemInfo.Any()) {
            WriteErrorMessage("Unable to detect any build systems in the specified repository, please try again.", exitCode: 1, exit: true);
        }

        WriteSuccessMessage($"Detected {buildSystemInfo.Count()} Build Systems.");

        await HandleCompilationAsync(settings, repoInfoObj, buildSystemInfo, selectedDockerImage);
        
        return 0;
            
    }
    
    private static async Task<bool> ExtractBuildFilesFromContainer(string containerTagName, string? repoName, string containerPath, string hostOutputDirectory)
    {

        if (repoName == null) {
            WriteWarningMessage("Unable to extract the build artifacts from the created container.");
            WriteErrorMessage("Variable 'repoName' is null in ExtractBuildFilesFromContainer()", exitCode: 1, exit: true);
        }

        if (!Directory.Exists(hostOutputDirectory)) {
            Directory.CreateDirectory(hostOutputDirectory);
        }

        // Creating a temp docker container.
        var createPSI = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"create --name simplecbuild-temp {containerTagName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var createContainerProcess = Process.Start(createPSI);
        
        if (createContainerProcess == null) {
            WriteWarningMessage("Unable to create the required docker container.");
            WriteErrorMessage("Variable 'createContainerProcess' is null", exitCode: 1, exit: true);
        }
        
        await createContainerProcess.WaitForExitAsync(); 
        

        // The active path within the container is based on the WorkDir Instruction.
        // The container path is either "build" (if Meson is in use) or "." otherwise.
        string fullContainerPath = $"/root/repos/{repoName}/{containerPath.TrimStart('/')}";
        
        // Sanitizing the container path to ensure consistency
        if (!fullContainerPath.EndsWith("/.")) {
            fullContainerPath += "/.";
        }

        // Copying the files out using the absolute path of the container
        var copySuccess = await CopyBuildArtifacts(fullContainerPath, hostOutputDirectory);
        
        // Deleting the temporary container now that execution has finished.
        await DeleteContainer();

        return copySuccess;
    }
    
    // Currently supports Autotools, Bazel, CMake, Make, and Meson.
    private static List<string> GetBuildCommands(BuildSystemInfo system)
    {
        return system.Name switch
        {
            "Autotools" => ["./configure", "make"],
            "Bazel" => ["bazel build"],
            "CMake" => ["cmake .", "make"],
            "Meson" => ["meson setup build", "meson compile -C build"],
            "Make" => ["make"],
            _ => []
        };
    }

    private static IEnumerable<BuildSystemInfo> GetBuildSystemInfo(RepoInfo repoInfo) 
    {
        var allSupportedSystems = BuildSystemLoader.GetBuildSystems();
        IEnumerable<BuildSystemInfo> foundSystems = [];

        var allFiles = repoInfo.TreeFiles;
        var rootFiles = allFiles.Where(f => !f.Contains('/'));

        foreach (var system in allSupportedSystems) 
        {
            if (system.IsPresent(allFiles, rootFiles)) {
                foundSystems = foundSystems.Append(system);
            }
        }
        return foundSystems;
    }
    private static string GetSystemExportPath(BuildSystemInfo system)
    {
        return system.Name switch
        {
            "Meson" => "build",
            _ => "." // Defaults to root if an unexpected system is provided.
        };
    }
    private static DockerImage GetSelectedDockerImage() 
    {
        var families = GetFamilies();


        // Choosing image family
        var familyNames = families.Select(fam => fam.Name);
        var familyChoice = AskForInput(
            message: "Please select your desired image family for the Docker container", 
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
        
        return selectedDockerImage;
    }
    private static async Task HandleCompilationAsync(
        [NotNull] MainMenuSettings settings, 
        RepoInfo repoInfoObj, 
        IEnumerable<BuildSystemInfo> buildSystemInfo, 
        DockerImage selectedDockerImage
    ) 
    {
        foreach (var buildSystem in buildSystemInfo) 
        {
            WriteStateMessage($"Attempting to compile using {buildSystem.Name}...");
            
            var buildCommands = GetBuildCommands(buildSystem);
            var buildInstructions = selectedDockerImage.GetInstructions(
                settings.RepoLink, 
                repoInfoObj.RepoUrlObj.RepoName,
                buildSystem.Installation,
                exportPath: $"/root/repos/{repoInfoObj.RepoUrlObj.RepoName}",
                buildCommands
            );

            var dockerFileContent = buildInstructions.Build();

            string tempDockerFilePath = Path.Combine(Path.GetTempPath(), $"Dockerfile.{buildSystem.Name}.{Guid.NewGuid()}");
            

            try 
            {
                File.WriteAllText(tempDockerFilePath, dockerFileContent);

                // The tag is needed here as the build artifacts will be moved when "FROM scratch ..." is called.
                string tagName = $"simplecbuild-{buildSystem.Name.ToLower()}-{Guid.NewGuid().ToString()[..8]}";

                bool success = await BuildContainer(tempDockerFilePath, buildSystem.Name, tagName);
                
                if (!success) {
                    WriteErrorMessage($"Failed to compile using {buildSystem.Name}, continuing.");
                }

                WriteSuccessMessage($"Successfully compiled repo using {buildSystem.Name}!");
                    
                // Attempting to extract build artifacts.
                string exportSourcePath = GetSystemExportPath(buildSystem);
                string hostOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output", buildSystem.Name);
                    
                WriteStateMessage($"Transferring compiled files from {exportSourcePath} to {hostOutputDirectory}...");
                
                if (await ExtractBuildFilesFromContainer(tagName, repoInfoObj.RepoUrlObj.RepoName, exportSourcePath, hostOutputDirectory)) {
                    WriteSuccessMessage($"Files transferred to {hostOutputDirectory}");
                    break;
                }
                
                WriteErrorMessage("Failed to transfer files, attempting to compile again.");
            }

            catch (Exception ex) {
                WriteWarningMessage("An exception occurred while attempting to compile the specified repository.");
                WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
            }

            finally 
            {
                if (File.Exists(tempDockerFilePath)) {
                    File.Delete(tempDockerFilePath);
                }
            }
        }

    }

    private static bool RepoLinkIsMissing([NotNull] MainMenuSettings settings) 
    {
        if (settings.RepoLink == null) {
            WriteErrorMessage("A link to a git repository must be specified.");
            WriteInformation("Use the --help flag for more information.");
            return true;
        }
        return false;
        
    }

    private static bool TokenOrPrivateFlagMissing([NotNull] MainMenuSettings settings) 
    {
        if (settings.PrivateFlagSet && settings.OAuthToken == null) 
        {
            WriteErrorMessage("When using the --private flag, an OAuth Token must be specified.");
            WriteInformation("Use the --help flag for more information.");
            return true;
        }

        if (!settings.PrivateFlagSet && settings.OAuthToken != null) 
        {
            WriteErrorMessage("Unexpected OAuth Token specified.");
            WriteWarningMessage("Please remove the token argument, or include --private in your command.");
            WriteInformation("Use the --help flag for more information.");
            return true;
        }
        return false;
    }

}