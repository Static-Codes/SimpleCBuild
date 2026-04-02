using DockerFileSharp.Common;
using DockerFileSharp.Common.Image;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the FROM Dockerfile Instruction. <br/>
///
///     The FROM instruction initializes a new build stage and sets the base image for subsequent instructions. <br/>
///     As such, a valid Dockerfile must start with a FROM instruction. <br/>
///     The image can be any valid image. <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#from
/// </summary>
/// 
/// <param name="ImageName"> 
///     The name of the docker image you want to run.
/// </param>
/// <param name="Alias"> 
///     An alias that you wish to assign to this image. 
/// </param>
/// <param name="Platform">
///     The platform identifier for the compiled binaries. <br/>
/// 
///     For more information execute: <br/>
///         docker buildx ls <br/>
/// </param>
// See docs/methodology.md for more information about implementating IDockerInstruction.
public record FromInstruction(string ImageName, string? Alias = null, string? Platform = null) : IDockerInstruction
{
    // Using an Image object to create a DockerImage object. 
    // Then using the newly created <DockerImage>.ImageName to create the FromInstruction object.
    public FromInstruction(IsoImage Image, string? Alias = null, string? Platform = null) : 
        this(new DockerImage(Image).ImageName, Alias, Platform) { }

    // Implementation of IDockerInstruction.Build()
    public string Build()
    {
        string platformPart = string.IsNullOrWhiteSpace(Platform) ? "" : $"--platform={Platform} ";
        string aliasPart = string.IsNullOrWhiteSpace(Alias) ? "" : $" AS {Alias}";
        return $"FROM {platformPart}{ImageName}{aliasPart}";
    }
}
