using System.Diagnostics;
using System.Text.RegularExpressions;
using static EasyDockerFile.Core.Helpers.ExecutableHelper;
using static EasyDockerFile.Core.Helpers.ProcessHelper;
using static Global.Logging;

namespace EasyDockerFile.Core.Inspections;


public partial class CMakeInspection 
{
    public static string Run(string cmakeInspectScriptPath, string projectDirectory) 
    {
        Process? process = null;
        
        var psi = new ProcessStartInfo() {
            FileName = GetPythonExecutable(),
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

        if (output.EndsWith("-- Unable to locate any CMake CodeModel*.json files.")) {
            return string.Empty;
        }

        var match = CMakeInspectionPathRegex().Match(output);
        
        if (!match.Success) {
            WriteWarningMessage("No CMake CodelModel JSON files were found.");
            return string.Empty;
        }

        return match.Value;
    }

    // The html tag is unused currently but will be replaced in the future.
    [GeneratedRegex(@"([a-zA-Z]:\\|[\\\/])[\w\-.\\\/]+?\.(?:json|html)")]
    private static partial Regex CMakeInspectionPathRegex();
}