using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary> 
///     Implementation of the ENTRYPOINT Dockerfile Instruction <br/>
/// 
///     An ENTRYPOINT allows you to configure a container that will run as an executable. <br/>
///
///     ENTRYPOINT has two possible forms: <br/>
///
///     1. The exec form, which is the preferred form: <br/> 
///         - ENTRYPOINT ["executable", "param1", "param2"] <br/>
/// 
///     2. The shell form: <br/>
///         - ENTRYPOINT command param1 param2 <br/>
/// 
///     For more information see: https://docs.docker.com/reference/dockerfile/#entrypoint
/// </summary>
/// 
/// <param name="Commands">
///     An array of individual command strings to be concatenated.
/// </param>
/// 
/// <param name="UseExecForm">
///     Determines whether or not to use the Exec form. <br/>
///     For more information see: https://docs.docker.com/reference/dockerfile/#exec-form
/// </param>

// See Docs/Methodology.md for more information about implementating IDockerInstruction.
public record EntryPointInstruction(string[] Commands, bool UseExecForm = true) : IDockerInstruction
{
    public string Build()
    {
        if (Commands.Length == 0) {
            return string.Empty;
        }

        if (UseExecForm) {
            string jsonParts = string.Join(", ", Commands.Select(c => $"\"{c}\""));
            return $"ENTRYPOINT [{jsonParts}]";
        }
        
        return $"ENTRYPOINT {string.Join(" ", Commands)}";
    }
}
