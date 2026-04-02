using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the RUN Dockerfile Instruction. <br/>
/// 
///     The RUN instruction will execute any commands to create a new layer on top of the current image. <br/>
/// 
///     The added layer is used in the next step in the Dockerfile. <br/>
/// 
///     RUN has two forms: <br/>
/// 
///     - Shell form: "RUN [OPTIONS] command ..." <br/>
///     
///     - Exec form:  "RUN [OPTIONS] [ "command", ... ]" <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#run
/// </summary>
/// 
/// <param name="Instruction">
///     The IDockerfileInstruction to be executed as a trigger action at build time.
/// </param>

// See docs/methodology.md for more information about implementating IDockerInstruction.
public record RunInstruction(params string[] Commands) : IDockerInstruction
{
    public string Build()
    {
        return Commands.Length switch {
            0 => string.Empty,
            1 => $"RUN {Commands[0]}",
            // 2 or more arguments passed.
            // Handles multiple commands using string formatting with a newline and horizontal tab
            _ => $"RUN {string.Join(" && \\\n\t", Commands)}"
        };
    }
}
