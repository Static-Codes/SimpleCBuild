using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Inspections;
using EasyDockerFile.Core.Types.Conversion;
using EasyDockerFile.Core.Types.Inspections.CMake;
using System.Diagnostics;
using System.Text.Json;
using static EasyDockerFile.Core.Helpers.ExecutableHelper;
using static EasyDockerFile.Core.Helpers.ProcessHelper;
using static EasyDockerFile.Core.Loaders.ConversionLoader;
using static EasyDockerFile.Core.Types.Inspections.CMake.CodeModelTypes;
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

    private void DeleteOldBuildArtifacts() 
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

            try {
                File.Delete(oldBuildArtifacts[i]);
            }
            catch (Exception ex) {
                WriteWarningMessage($"Unable to delete file at: {oldBuildArtifacts[i]}");
                WriteErrorMessage(ex.Message);
                continue;
            }

            WriteInformation($"Deleted old build artifact at: {oldBuildArtifacts[i]} ({i+1}/{oldBuildArtifacts.Length})");
            Thread.Sleep(350);
        }

        var cmakeBuildDirectory = Path.Combine(ProjectDirectory, "build", ".cmake");

        if (Directory.Exists(cmakeBuildDirectory)) {
            try {
                Directory.Delete(cmakeBuildDirectory, recursive: true);
            }
            catch (Exception ex) {
                WriteWarningMessage($"Unable to delete directory at: {cmakeBuildDirectory}");
                WriteErrorMessage(ex.Message);
                return;
            }
        }

        WriteSuccessMessage($"Deleted all old build artifacts from: {ProjectDirectory}");
    }

    private AutotoolsConversionResponse TranslateToCMake() 
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
            RootCMakeListsPath =  cmakeListFileExists ? cmakeListFilePath : "Not Generated",
            // No explicit null check is required.
            CMakeInspectPath = AutotoolsResources.CMakeInspectPath!
        };
    }    

    public List<CodeModelReplyTarget.Root>? TranslateToCMakeAndInspect() 
    {
        var conversionResponse = TranslateToCMake();

        if (!conversionResponse.Completed) {
            WriteErrorMessage($"The Autotools to CMake conversion failed for: {ProjectDirectory}", exitCode: 1, exit: true);
        }

        // This returns a list of all CodeModel-v2-{uuid}.json files returned by cmake_inspect.py
        var codeModelJSONFile = CMakeInspection.Run(conversionResponse.CMakeInspectPath, ProjectDirectory);


        if (string.IsNullOrEmpty(codeModelJSONFile)) {
            Console.WriteLine("The CMake CodeModel JSON file could not located, deserialization is not possible.");
            Environment.Exit(1);
        }

        var codeModelStream = new FileStream(codeModelJSONFile, FileMode.Open, FileAccess.Read);

        var codeModelData = JsonSerializer.Deserialize<CodeModelRoot>(codeModelStream);

        if (codeModelData == null) {
            Console.WriteLine("CMake CodeModel JSON data is null, deserialization is not possible.");
            Environment.Exit(1);
        }

        var processedModelList = CodeModelProcessor.ProcessCodeModel(codeModelData);


        return processedModelList;

    }
    
}