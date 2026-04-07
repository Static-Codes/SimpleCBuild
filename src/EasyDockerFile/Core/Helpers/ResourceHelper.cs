using EasyDockerFile.Core.API.PackageSearch;
using static Global.Constants;
using static Global.Logging;

namespace EasyDockerFile.Core.Helpers;

public class ResourceHelper 
{
    private static string GenerateFileName(bool isPythonFile) 
    {
        var fileExtension = isPythonFile ? "py" : "tmp";
        var fileName = $"simplecbuild-{Guid.NewGuid()}.{fileExtension}";
        return Path.Combine(Path.GetTempPath(), fileName);
    }


    /// <summary>
    /// Fetches the resource at the specified path, writes the stream to disk, and returns the temporary filepath.
    /// </summary>
    
    public static string WriteEmbeddedResourceToDisk(string resourcePattern, bool isExecutable = false, bool isPythonFile = false) 
    {
        string? fileName = null;

        Stream? resourceStream = null;
        
        try 
        { 
            resourceStream = _assembly.GetManifestResourceStream(resourcePattern);

            if (resourceStream == null) {
                WriteWarningMessage($"Unable to write resource: '{resourcePattern}' to disk.");
                WriteErrorMessage("Variable 'resourceStream' is null.", exitCode: 1, exit: true);
            }
            
            fileName = GenerateFileName(isPythonFile);

            // Ensuring the file doesn't already exist prior to continuing.
            while (File.Exists(fileName)) {
                fileName = GenerateFileName(isPythonFile);
            }

            using var fileStream = new FileStream(fileName, FileMode.CreateNew);
            resourceStream.CopyTo(fileStream);

            if (!isExecutable) {
                return fileName;
            }
            
            bool execPermSet = false;

            if (OperatingSystem.IsWindows()) {
                return Path.Combine(TEMP_DIR, fileName);
            }

            else if (OperatingSystem.IsLinux()) {
                execPermSet = UnixFilePermissions.SetExecutablePermissions(fileName);
            }

            if (!execPermSet) {
                WriteWarningMessage($"Unable to set executable permissions for temporary utility file: '{fileName}'");
                WriteErrorMessage("execPermSet is false in WriteEmbeddedResourceToDisk()", exitCode: 1, exit: true);
            } 

            WriteSuccessMessage($"Executable permissions set for temporary utility file: '{fileName}'");
        }

        catch (Exception ex) 
        {
            WriteWarningMessage($"Unable to write resource: '{resourcePattern}' to disk.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }

        finally 
        {
            if (resourceStream != null && resourceStream.CanTimeout) {
                resourceStream.Dispose();
            }
        }

        return fileName;
    }
}