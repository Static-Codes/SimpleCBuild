using System.Diagnostics;
using static EasyDockerFile.Core.Helpers.ProcessHelper;
using static Global.Logging;

namespace EasyDockerFile.Core.Inspection;


public class CMakeInspection 
{
    public static string[] Run(string cmakeInspectScriptPath, string projectDirectory) 
    {
        Process? process = null;
        
        var psi = new ProcessStartInfo() {
            FileName = "python3",
            Arguments = $"{cmakeInspectScriptPath} {projectDirectory}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false,
        };

        ReassignAndRunProcess(psi, ref process);

        if (process.ExitCode != 0) {
            WriteWarningMessage($"Unable to run CMake inspection in: {projectDirectory}");
            var errorString = process.StandardError.ReadToEnd();
            var error = !string.IsNullOrEmpty(errorString) ? errorString : $"process exited with code {process.ExitCode}"; 
            WriteErrorMessage(error, exitCode: 1, exit: true);
        }

        var output = process.StandardOutput.ReadToEnd();
        
        if (string.IsNullOrEmpty(output)) {
            WriteWarningMessage($"Unable to run CMake inspection in: {projectDirectory}");
            WriteErrorMessage("The process returned no output.", exitCode: 1, exit: true);
        }

        var paths = output.Split("| ");
        
        foreach (var path in paths) {
            Console.WriteLine(path);
        }
        return paths;
    }
}