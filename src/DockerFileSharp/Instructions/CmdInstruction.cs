using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;


/// <summary> 
///     Implementation of the CMD Dockerfile Instruction <br/>
///     Not to be confused with RunInstruction which actually exectutes a command. <br/>
///     For more information see: https://docs.docker.com/reference/dockerfile/#cmd
/// </summary>
/// <param name="Commands">
///     An array of individual command strings to be concatenated.
/// </param>
/// <param name="UseExecForm">
///     Determines whether or not to use the Exec form. <br/>
///     For more information see: https://docs.docker.com/reference/dockerfile/#exec-form
/// </param>


// See Docs/Methodology.md for more information about implementating IDockerInstruction.
public record CmdInstruction(string[] Commands, bool UseExecForm = true) : IDockerInstruction
{
    public string Build()
    {
        if (Commands.Length == 0) {
            return string.Empty;
        }

        if (UseExecForm) {
            string jsonParts = string.Join(", ", Commands.Select(c => $"\"{c}\""));
            return $"CMD [{jsonParts}]";
        }
        
        return $"CMD {string.Join(" ", Commands)}";
    }
}
