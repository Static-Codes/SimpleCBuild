using System.Diagnostics;

namespace EasyDockerFile.Core.Extensions;

public static class ProcessExtensions 
{
    /// <summary>
    /// Performs a null check and ensures this.HasExited is false.
    /// <br/> If these conditions are met, the process is disposed of.
    /// </summary>
    public static void DisposeSafely(this Process? process) 
    {
        if (process != null && !process.HasExited) {
            process.Dispose();
        }
    }
}
