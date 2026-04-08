using static EasyDockerFile.Core.Common.Platform;

namespace EasyDockerFile.Core.Helpers;

public static class ExecutableHelper
{
    public static string GetPythonExecutable() 
    {
        return IsWindows switch {
            true => "python",
            false => "python3"
        };
    }


    public static string GetShellArg() 
    {
        return IsWindows switch {
            true => "/c",
            false => "-c"
        };
    }

    public static string GetShellExecutable() 
    {
        return IsWindows switch {
            true => "cmd.exe",
            false => "/bin/bash"
        };
    }

    public static string GetWhichExecutable() 
    {
        return IsWindows switch {
            true => "where",
            false => "which"
        };
    }
}