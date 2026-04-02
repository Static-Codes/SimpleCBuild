using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the USER Dockerfile Instruction. <br/>
/// 
///     The USER instruction sets the username/UID and optionally the user-group/GID. <br/>
///     
///     This instruction can be set before any of the following instructions: <br/>
///     
///     - CMD <br/>
///     - ENTRYPOINT <br/>
///     - RUN <br/>
///     
///     It then then be used in the proceeding instruction(s). <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#user
/// </summary>
/// <param name="User">
///     [Required]: The username/UID you wish to set.
/// </param>
/// <param name="Group">
///     [Optional]: The group-name/GID you wish to set.
/// </param> 


// See docs/methodology.md for more information about implementating IDockerInstruction.
public record UserInstruction(string User, string? Group = null) : IDockerInstruction
{
    public string Build()
    {
        return (Group == null) switch {
            true => $"USER {User}",
            false => $"USER {User}:{Group}"
        };
    }
}
