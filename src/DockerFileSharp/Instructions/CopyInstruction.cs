using System.Text;
using DockerFileSharp.Common;
using static DockerFileSharp.Common.Entensions;

namespace DockerFileSharp.Instructions;

/// <summary> 
///     Implementation of the COPY Dockerfile Instruction <br/>
/// 
///     The ADD and COPY instructions are functionally similar, but serve slightly different purposes. <br/>
///     For more information see: https://docs.docker.com/build/building/best-practices/#add-or-copy  <br/>
/// 
///     Note: This implementation doesn't include: <br/>
/// 
///     <br/>--link, --parents, --exclude
///     <br/>For more information see: https://docs.docker.com/reference/dockerfile/#copy
/// </summary>
/// 
/// <param name="Source">
///     The source file or directory to use in the COPY Instruction. <br/>
///     You can specify multiple source files or directories with COPY, however: <br/>
///     The last argument must always be the destination.
/// </param>
/// 
/// <param name="Destination">
///     The destination file or directory to use in the COPY Instruction. <br/>
///     If the destination path begins with a forward slash: <br/>
///         <br/>- It's interpreted as an absolute path
///         <br/>- The source files are copied into the specified destination relative to the root of the current build stage.<br/>
/// </param>
/// 
/// <param name="From">
///     By default, the COPY instruction copies files from the build context. <br/>
///     The COPY --from flag lets you copy files from an image, a build stage, or a named context instead.
/// </param>
/// 
/// <param name="Chmod">
///     The --chmod flag supports octal notation (e.g., 755, 644) and symbolic notation (e.g., +x, g=u). <br/>
///     Symbolic notation (added in Dockerfile version 1.14) is useful when octal isn't flexible enough. <br/>
///     For example, u=rwX,go=rX sets directories to 755 and files to 644, while preserving the executable bit on files that already have it. <br/>
///     (Capital X means "executable only if it's a directory or already executable.") <br/>
///     
///     Example: <br/>
///     CopyInstruction(Source: "app.sh", Destination: "/app/", Chmod: "755"); <br/>
///
///     Returns: <br/>
///     COPY --chmod=755 app.sh /app/
///
///    
/// </param>

// See Docs/Methodology.md for more information about implementating IDockerInstruction.
public record CopyInstruction(string Source, string Destination, string? From = null, string? Chmod = null, string? Chown = null) 
: IDockerInstruction
{   
    
    private readonly string?[] CopyOptionValues = [From, Chmod, Chown];
    public string Build()
    {
        var extraOptions = CopyOptionValues?.Where(option => option != null) ?? [];
        
        // Handles cases where no extra arguments are provided.
        if (!extraOptions.Any()) {
            return $"COPY {Source} {Destination}";
        }

        var builder = new StringBuilder();
        builder.Append("COPY ");

        if (extraOptions.Contains(From)) {
            builder.Append($"--from={From} ");
        }

        if (extraOptions.Contains(Chmod)) {
            builder.Append($"--from={Chmod} ");
        }

        if (extraOptions.Contains(Chmod)) {
            builder.Append($"--chmod={Chmod} ");
        }

        if (extraOptions.Contains(Chown)) {
            builder.Append($"--chown={Chown} ");
        }

        builder.Append($"{Source} {Destination}");
        return builder.ToString();
    }
}
