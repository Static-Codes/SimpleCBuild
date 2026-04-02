using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the ONBUILD Dockerfile Instruction. <br/>
/// 
///     The ONBUILD instruction adds a trigger instruction to the image. <br/>
///     This trigger instruction will be executed when the image is reused at another time. <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#onbuild
/// </summary>
/// <param name="Instruction">
///     The IDockerfileInstruction to be executed as a trigger action at build time.
/// </param>
// See docs/methodology.md for more information about implementating IDockerInstruction.
public record OnBuildInstruction(IDockerInstruction Instruction) : IDockerInstruction
{
    public string Build()
    {
        string innerBuild = Instruction.Build();
        
        if (innerBuild.IsEmpty()) {
            return string.Empty;
        }

        return $"ONBUILD {innerBuild}";
    }
}
