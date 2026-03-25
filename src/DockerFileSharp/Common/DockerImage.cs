using DockerFileSharp.Common.Image;
using DockerFileSharp.Instructions;
using Global.Build;
using static Global.Logging;

namespace DockerFileSharp.Common;

public class DockerImage(IsoImage selectedImage) 
{
    public string ImageName => GetImageName(selectedImage);

    private static string GetImageName(IsoImage Image)
    {
        // Currently this is excessive as full name could be parsed.
        // However, with future maintenance in mind, this is a more robust solution. 
        var baseName = Image.FullName.Split(' ')[0].ToLower();
        var version = Image.Version.ToLower();

        return $"{baseName}:{version}";
    }

    /// <summary> 
    ///     Builds a List of IDockerInstruction(s) 
    /// </summary>
    /// 
    /// <param name="repoLink">
    ///     A link to the repository which will be cloned within the container 
    /// </param>
    /// 
    /// <param name="repoName">
    ///     <br/> The name of the repository which will be cloned. 
    ///     <br/> This is used in the command:
    ///     <br/> cd "repoName"
    /// </param>
    /// 
    /// <param name="installation">
    ///     The BuildSystemInstallation object containing the installation instructions for the build system.
    /// </param>
    /// 
    /// <param name="sourceDir">
    ///     <br/> The path on the container to a directory that will be extracted after compilation finishes. 
    ///     <br/> Defaults to the root directory (will copy the entire container which will be 4.7GB, this is not recommended.)
    /// </param>
    /// 
    /// <param name="buildCommands">
    ///     <br/> A list of build commands loaded from BuildSystems.xml
    /// </param>
    /// 
    public List<IDockerInstruction> GetInstructions(string? repoLink, string? repoName, BuildSystemInstallations installations, string? sourceDir = "/", BuildSystemCommands? buildCommands = null) 
    {
        if (repoName == null) {
            WriteWarningMessage("Unable to return the dockerfile instructions that are required to continue.");
            WriteErrorMessage("repoName is null in DockerImage.GetInstructions()");
            return [];
        }

        if (repoLink == null) {
            WriteWarningMessage("Unable to return the dockerfile instructions that are required to continue.");
            WriteErrorMessage("repoLink is null in DockerImage.GetInstructions()");
            return [];
        }

        var repoPath = $"/root/repos/{repoName}";

        List<IDockerInstruction> Instructions = [
            new FromInstruction(ImageName, Alias: "build")
        ];

        if (ImageName.StartsWith("debian:")) 
        {
            Instructions.AddRange(  
            [
                new EnvInstruction(
                    new Dictionary<string, string> {
                        { "DEBIAN_FRONTEND", "noninteractive" }
                    }
                ),

                new RunInstruction([
                    "apt-get update",
                    "apt-get install git curl -y",
                    "mkdir -p /root/repos",

                ]),
            ]);
        }

        else {
            WriteWarningMessage("Unable to return the dockerfile instructions that are required to continue.");
            WriteErrorMessage("ImageName returned an unexpected value in DockerImage.GetInstructions()");
            Console.WriteLine($"[VALUE]: {ImageName}");
            return [];
        }

        // Adding required packages for the build system itself.
        Instructions.Add(new RunInstruction(GetPackagesForBuildSystem(installations)));

        // Cloning and working directory instructions
        Instructions.AddRange(
        [
            new WorkDirInstruction("/root/repos"),

            new RunInstruction([$"git clone {repoLink}"]),

            new WorkDirInstruction(repoPath)
        ]);

        // Handling build commands.
        if (buildCommands != null)
        {
            Instructions.Add(
                new RunInstruction(
                    [.. GetCommandsForBuildSystem(buildCommands)]
                )
            );
        }


        // Copying the build artifacts from container to the host machine.
        if (!string.IsNullOrEmpty(sourceDir))
        {
            Instructions.AddRange([
                new FromInstruction("scratch"),
                new CopyInstruction(
                    Source: sourceDir, 
                    Destination: "/", 
                    From: "build"
                )
            ]);
        }

        return Instructions;
    }

    private static string[] GetCommandsForBuildSystem(BuildSystemCommands buildCommands) 
    {
        #if DEBUG
            Console.WriteLine(buildCommands);
        #endif
        
        if (buildCommands.Debian != null && buildCommands.Debian.BuildCommands.Count > 0) {
			return [.. buildCommands.Debian.BuildCommands];
		}
		return [];
    }

    private string[] GetPackagesForBuildSystem(BuildSystemInstallations installations) 
    {
        var distroName = ImageName.Split(':')[0].ToLower();

        #if DEBUG
            WriteDebugMessage(installations.ToString());
        #endif

        
        if (installations.Debian != null && installations.Debian.InstallationCommands.Count > 0) {
			return [.. installations.Debian.InstallationCommands];
		}
		return ["apt-get install -y build-essential cmake"];
		
    }
}
