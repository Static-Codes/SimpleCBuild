using System.Runtime.InteropServices;

namespace EasyDockerFile.Core.Extensions;

public static class StringExtension
{
    public static Architecture ToArchitecture(this string archString) => archString switch {
        "x64" => Architecture.X64,
        "arm64" => Architecture.Arm64,
        "armhf" or "armel" => Architecture.Arm,
        _ => throw new InvalidOperationException($"Unable to parse '{archString}' to System.Runtime.InteropServices.Architecture")
    };
}