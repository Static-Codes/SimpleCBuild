using static EasyDockerFile.Core.Common.Platform;

namespace EasyDockerFile.Core.Types.Generic;

public static class ExecutableHelper
{
    public static string GetDockerExecutable() 
    {
        // Windows executable name pulled from:
        // https://forums.docker.com/t/docker-credential-desktop-exe-executable-file-not-found-in-path-using-wsl2/100225/13
        return IsWindows switch {
            true => "docker-credential-desktop.exe",
            false => "docker-desktop"
        };
    }

    public static string GetPythonExecutable() 
    {
        return IsWindows switch {
            true => "python",
            false => "python3"
        };
    }
}