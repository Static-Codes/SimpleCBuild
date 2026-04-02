using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the STOPSIGNAL Dockerfile Instruction. <br/>
/// 
///     The STOPSIGNAL instruction sets the system call signal that will be sent to the container to exit. 
///     This signal can be a signal name in the format SIG<NAME>, 
///     
///     - Example: SIGKILL, 
///     
///     Also supports an unsigned number that matches a position in the kernel's syscall table
///     - Example: 9 
///     
///     The default signal is SIGTERM, if no other definition is found.
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#stopsignal
/// </summary>
/// 
/// <param name="Signal">
///     The signal to send to the container instance:
///     - Examples:
///     - "SIGTERM"
///     - "9"
/// </param>

// See docs/methodology.md for more information about implementating IDockerInstruction.
public record StopSignalInstruction(string Signal) : IDockerInstruction
{
    public string Build() {
        return $"STOPSIGNAL {Signal}";
    }
}
