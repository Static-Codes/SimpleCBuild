using static EasyDockerFile.Core.Common.Platform;
using static EasyDockerFile.Core.Common.Constants;
using static EasyDockerFile.Core.Common.SessionInfo;
using static System.Runtime.InteropServices.Architecture;

namespace EasyDockerFile.Core.Helpers;

public class ZChunkLoader() 
{
    private static string GetResourcePattern() 
    {
        var check = (HostArchitecture, IsWindows, IsMac, IsLinux, IsBSD);
        
        return check switch {
            (X64, true, _, _, _)  => $"{BaseZChunkPattern}.win_x64.native.unzck.exe",
            (X64, _, true, _, _)  => $"{BaseZChunkPattern}.osx_universal.native.unzck",
            (X64, _, _, true, _)  => $"{BaseZChunkPattern}.linux_x64.native.unzck",
            (X64, _, _, _, true)  => $"{BaseZChunkPattern}.linux_x64.native.unzck",

            (Arm64, _, true, _, _)  => $"{BaseZChunkPattern}.osx_universal.native.unzck",
            (Arm64, _, _, true, _)  => $"{BaseZChunkPattern}.linux_arm64.native.unzck",
            (Arm64, _, _, _, true)  => $"{BaseZChunkPattern}.linux_arm64.native.unzck",
            _ => throw new PlatformNotSupportedException("Expected an x64 or ARM64 host machine in GetResourcePattern()")
        };
    }

    public static string? Load() 
    {
        var pattern = GetResourcePattern();
        try 
        {
            var stream = _assembly.GetManifestResourceStream(pattern);
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            using var fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true);
            stream.CopyTo(fileStream);
            return fileName;
        }
        catch (Exception ex) 
        {
            Console.WriteLine("[WARNING]: Unable to load unzck binary");
            Console.WriteLine($"[ERROR TYPE]: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
        }
        return null;
    }

    
}