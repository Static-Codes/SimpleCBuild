using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;


/// <summary>
///     Implementation of the WORKDIR Instruction. <br/>
///     
///     The WORKDIR instruction sets the working directory for the proceeding instruction types: <br/>
///         - ADD <br/>
///         - CMD <br/>
///         - COPY <br/>
///         - ENTRYPOINT <br/>
///         - RUN <br/>
///     The WORKDIR instruction can be used multiple times in a single DOCKERFILE.  <br/>
///     
///     If a relative path is provided, it will be relative to the path of the previous WORKDIR instruction. <br/>
/// 
///     For example: <br/>
///         - WORKDIR /a -> Sets the current working directory to "/a/" <br/>
///         - WORKDIR b  -> Sets the current working directory to "/a/b/" <br/>
///         - WORKDIR c  -> Sets the current working directory to "/a/b/c" <br/>
///         - RUN pwd    -> Executes the pwd command which will output "/a/b/c" <br/>
/// 
///     General Usage: <br/>
///         - WORKDIR /path/to/workdir <br/>
/// </summary>
/// 
/// <param name="Path">
///     The relative or absolute path you wish to use as the current working directory for the following instructions.
/// </param>

// See docs/methodology.md for more information about implementating IDockerInstruction.
public record WorkDirInstruction(string Path) : IDockerInstruction
{
    public string Build() {
        return $"WORKDIR {Path}";
    }
}
