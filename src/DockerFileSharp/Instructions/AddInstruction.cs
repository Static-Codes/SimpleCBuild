using DockerFileSharp.Common;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the ADD Dockerfile Instruction. <br/>
/// 
///     The ADD instruction:
///     - Copies new files, directories or remote file URLs. <br/>
///     - Adds them to the filesystem of the image at the destination path. <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#add
/// </summary>
/// <param name="Source">
///     [Required]: The source file, directory, or remote file URL you wish to add to the DockerImage's filesystem.
/// </param>
/// <param name="Destination">
///     [Required]: The destination path for the copied resource in the current DockerImage.
/// </param>
/// <param name="Chown">
///     [Optional]: The UserGroup ownership string, example: "user:group".
/// </param>
/// <param name="Chmod">
///     [Optional]: The desired File permissions, example: "755". <br/>
///     [Resource]: https://help.rc.unc.edu/how-to-use-unix-and-linux-file-permissions/
/// </param>
/// <param name="Checksum">
///     [Optional]: The checksum of the remote URL, if provided.
/// </param>
// See docs/methodology.md for more information about implementating IDockerInstruction.
public record AddInstruction(string Source, string Destination, string? Chown = null, string? Chmod = null, string? Checksum = null) : IDockerInstruction
{
    public string Build()
    {
        var options = new List<string>();
        if (!Chown.IsEmpty()) {
            options.Add($"--chown={Chown}");
        }

        if (!Chmod.IsEmpty()) {
            options.Add($"--chmod={Chmod}");
        }

        if (!Checksum.IsEmpty()) {
            options.Add($"--checksum={Checksum}");
        }

        if (options.Count == 0) {
            return $"ADD {Source} {Destination}";
        }
        
        return $"ADD {string.Join(" ", options)} {Source} {Destination}";
    }
}
