using DockerFileSharp.Common;
using System.Text;

namespace DockerFileSharp.Instructions;

/// <summary>
///     Implementation of the LABEL Dockerfile Instruction. <br/>
/// 
///     The LABEL instruction adds metadata to an image. 
///     
///     These Labels are key-value pairs of the type Dictionary<string, string> <br/>
///     To include spaces within a LABEL value, use quotes and backslashes as you would in command-line parsing. <br/>
/// 
///     For more information, see: https://docs.docker.com/reference/dockerfile/#label
/// </summary>
/// 
/// <param name="Labels">
///     A Dictionary containing the key and value pairs for an IsoImage object's metadata.
/// </param>

// See docs/methodology.md for more information about implementating IDockerInstruction.
public record LabelInstruction(Dictionary<string, string> Labels) : IDockerInstruction
{
    public string Build()
    {
        if (Labels.Count == 0) return string.Empty;

        var builder = new StringBuilder();
        builder.Append("LABEL ");

        foreach (var pair in Labels)
        {
            builder.Append($"{pair.Key}={pair.Value} ");
        }

        return builder.ToString().TrimEnd();
    }
}
