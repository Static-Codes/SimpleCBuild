using DockerFileSharp.Common.Image;
using DockerFileSharp.Instructions;
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

    public List<IDockerInstruction> GetInstructions(string? repoLink, string? repoName, List<string>? buildCommands = null) 
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

        List<IDockerInstruction> Instructions = [
            new FromInstruction(ImageName)
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

        else if (ImageName.StartsWith("fedora:")) 
        {
            Instructions.AddRange(
            [
                new RunInstruction([
                    "dnf update",
                    "dnf install git curl -y",
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

        Instructions.AddRange(
        [
            new WorkDirInstruction("/root/repos"),

            new RunInstruction([$"git clone {repoLink}"]),

            new WorkDirInstruction($"/root/repos/{repoName}")
        ]);

        if (buildCommands != null && buildCommands.Count > 0)
        {
            Instructions.Add(
                new RunInstruction(
                    [..buildCommands]
                )
            );
        }

        return Instructions;
    }
}
