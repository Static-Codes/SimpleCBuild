namespace EasyDockerFile.Core.Helpers;

public static class ExecutableHelper
{
    public static string GetPythonExecutable() 
    {
        return OperatingSystem.IsWindows() switch {
            true => "python",
            false => "python3"
        };
    }


    public static string GetShellArg() 
    {
        return OperatingSystem.IsWindows() switch {
            true => "/c",
            false => "-c"
        };
    }

    public static string GetShellExecutable() 
    {
        return OperatingSystem.IsWindows() switch {
            true => "cmd.exe",
            false => "/bin/bash"
        };
    }

    public static string GetWhichExecutable() 
    {
        return OperatingSystem.IsWindows() switch {
            true => "where",
            false => "which"
        };
    }
}