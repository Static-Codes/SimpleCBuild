using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the EXPOSE Dockerfile Instruction. <br/>
/// 
///     The EXPOSE instruction informs Docker that the container listens on the specified network ports at runtime. <br/>
///     You can specify whether the port listens on TCP or UDP, and the default is TCP if the protocol is not specified. <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#expose
/// </summary>
///
/// <param name="Ports">
///     A string array of Ports you wish to expose on the container at runtime. <br/>
///     - Example: ["80", "80/tcp", "443/udp"]
/// </param>

// See docs/methodology.md for more information about implementating IDockerInstruction.
public record ExposeInstruction(params string[] Ports) : IDockerInstruction
{
    public string Build()
    {
        return Ports.Length switch
        {
            0 => string.Empty,
            _ => $"EXPOSE {string.Join(" ", Ports)}"
        };
    }
}
