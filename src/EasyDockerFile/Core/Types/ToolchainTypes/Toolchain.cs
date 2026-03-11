namespace EasyDockerFile.Core.Types.ToolchainTypes;


public enum ToolchainName 
{
    Autotools = 0,
    Bazel = 1,
    CMake = 2,
    Make = 3,
    Meson = 4,
    MSBuild = 5,
    Ninja = 6,
}


public class Toolchain 
{
    public required BuildSystemName BuildSystem;
    public string? MinimumVersionSupported { get; set; }
    public required PackageManagerName[] PackageManagers { get; set; }
    public Dictionary<string, string>? SubModules { get; set; }
    public CompilerName? CompilerName { get; set; }
}