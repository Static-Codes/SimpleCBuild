using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Types.Conversion;
using System.Diagnostics;
using static EasyDockerFile.Core.Helpers.ExecutableHelper;
using static EasyDockerFile.Core.Helpers.ProcessHelper;
using static EasyDockerFile.Core.Loaders.ConversionLoader;
using static Global.Logging;


namespace EasyDockerFile.Core.Conversion;


/// <summary>
/// Converts an Autotools project at the specific directory to a CMake project.
/// </summary>
/// 
/// <param name="projectDirectory">
/// The directory containing an Autotools project.
/// </param>
/// 
/// <returns>
/// If CMakeLists is created:
///     <br/> (success: true, CMakeListsFilePath: "path/to/CMakeLists.txt") <br/> 
/// If the conversion failed: 
///     <br/> (success: false, CMakeListsFilePath: "Not Generated")
/// </returns>
public class AutotoolsConverter(string projectDirectory)
{
    public string ProjectDirectory { get; init; } = projectDirectory;
    AutotoolsResources AutotoolsResources { get; init; } = LoadAutotoolsResources();
    
    private string[] FindOldBuildArtifacts() 
    {
        List<string> oldBuildArtifacts = [];

        var rootCMakeFile = Path.Combine(ProjectDirectory, "CMakeLists.txt");
        
        if (File.Exists(rootCMakeFile)) {
            oldBuildArtifacts.Add(rootCMakeFile);
        }
        
        var subDirectories = Directory.GetDirectories(ProjectDirectory);

        foreach (var subDirectory in subDirectories) 
        {

            var subDirectoryCMakeFile = Path.Combine(subDirectory, "CMakeLists.txt");

            if (File.Exists(subDirectoryCMakeFile)) {
                oldBuildArtifacts.Add(subDirectoryCMakeFile);
            }
        }

        return [.. oldBuildArtifacts];
    }

    public void DeleteOldBuildArtifacts() 
    {
        var oldBuildArtifacts = FindOldBuildArtifacts();

        if (oldBuildArtifacts.Length == 0) {
            WriteWarningMessage("No old build artifacts were located, continuing.");
            return;
        }

        WriteInformation("Deleting old build artifacts.");
        Thread.Sleep(300);

        for (int i = 0; i < oldBuildArtifacts.Length; i++) 
        {
            Thread.Sleep(350);

            File.Delete(oldBuildArtifacts[i]);
            WriteInformation($"Deleted old build artifact at: {oldBuildArtifacts[i]} ({i}/{oldBuildArtifacts.Length})");
            Thread.Sleep(350);
        }

        WriteSuccessMessage($"Deleted all build artifacts from: {ProjectDirectory}");
    }

    public AutotoolsConversionResponse TranslateToCMake() 
    {
        DeleteOldBuildArtifacts();

        Process? process = null;

        var auto2CMakePSI = new ProcessStartInfo() 
        {
            FileName = $"{GetPythonExecutable()}",
            Arguments = $"{AutotoolsResources.Auto2CMakePath} -d {ProjectDirectory}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false,
        };

        try 
        {
            ReassignAndRunProcess(auto2CMakePSI, ref process);

            if (process.ExitCode != 0) {
                WriteWarningMessage("Unable to convert the provided Autotools project to CMake.");
                WriteErrorMessage(
                    message: $"Command '{process.StartInfo.FileName} {process.StartInfo.Arguments}' exited was code {process.ExitCode}", 
                    exitCode: 1,
                    exit: true
                );
            }

            WriteSuccessMessage("Converted Autotools project to CMake project!"); 

        }

        catch (Exception ex) 
        {
            WriteWarningMessage("Unable to convert the provided Autotools project to CMake.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }

        finally 
        {
            process.DisposeSafely();

            // Now that this resource has been used, it can be safely deleted.
            if (File.Exists(AutotoolsResources.Auto2CMakePath)) {
                File.Delete(AutotoolsResources.Auto2CMakePath);
                AutotoolsResources.Auto2CMakePath = null;
            }
        }

        var cmakeListFilePath = Path.Combine(ProjectDirectory, "CMakeLists.txt");
        var cmakeListFileExists = File.Exists(cmakeListFilePath);

        var inspectPathIsNull = AutotoolsResources.CMakeInspectPath == null;

        // Handling a VERY unlikely edge case, better safe than sorry! :)
        // If cmakeListFileExists is true than inspectPathIsNull should ALWAYS be false.
        if (cmakeListFileExists && inspectPathIsNull) {
            WriteSuccessMessage($"CMakeLists generated at: {cmakeListFilePath}");
            WriteErrorMessage("Failed to load cmake_inspect.py", exitCode: 1, exit: true);
        }

        if (!cmakeListFileExists) {
            WriteWarningMessage("Failed to convert Autotools project to CMake.");
            WriteErrorMessage("cmakeListFileExists is false in AutotoolsConversionResponse.TranslateToCMake().", exitCode: 1, exit: true);
        }


        return new() {
            Completed = cmakeListFileExists,
            CMakeListsFilePath =  cmakeListFileExists ? cmakeListFilePath : "Not Generated",
            // No explicit null check is required.
            CMakeInspectPath = AutotoolsResources.CMakeInspectPath!
        };
    }    


}