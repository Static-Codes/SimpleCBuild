using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the SHELL Dockerfile Instruction. <br/>
/// 
///     The SHELL instruction allows the default shell used for the shell form of 
///     commands to be overridden. <br/>
///     
///     Default Shells:
///     - Linux: ["/bin/sh", "-c"] <br/>
///     - Windows ["cmd", "/S", "/C"] <br/>
///     
///     For more information, see: https://docs.docker.com/reference/dockerfile/#shell
/// </summary>
/// <param name="Shell">
///     The shell and its arguments <br/>
///     Windows Example: ["powershell", "-Command"]
///     Unix Example: ["/bin/bash", "-c Command"]
/// </param>
// See docs/methodology.md for more information about implementating IDockerInstruction.
public record ShellInstruction(params string[] Shell) : IDockerInstruction
{
    public string Build() 
    {
        return Shell.Length switch {
            0 => string.Empty,
            _ => $"SHELL [{string.Join(", ", Shell.Select(s => $"{s}"))}]"
        };
    }
}
