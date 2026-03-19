using System.Runtime.InteropServices;
using System.Text;

namespace DockerFileSharp.Common;

public static class Extensions 
{
    public static bool IsEmpty(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Extension method used to map an architecture string to the corresponding System.Runtime.InteropServices.Architecture member.
    /// </summary>
    public static Architecture ToArchitecture(this string archString) => archString switch {
        "x64" => Architecture.X64,
        "arm64" => Architecture.Arm64,
        "armhf" or "armel" => Architecture.Arm,
        _ => throw new InvalidOperationException($"Unable to parse '{archString}' to System.Runtime.InteropServices.Architecture")
    };

    /// <summary>
    /// Extension method used to create a new line seperated string based on the provided list of instructions.
    /// </summary>
    public static string Build(this List<IDockerInstruction> instructions) {
        var builder = new StringBuilder();
        instructions.ForEach(i => builder.AppendLine(i.Build()));
        return builder.ToString();
    }
}
