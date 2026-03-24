using Global.Build;

namespace EasyDockerFile.Core.Extensions;

public static class ToolchainExtension 
{
    public static string GetName(this ToolchainName name) => nameof(name);
}