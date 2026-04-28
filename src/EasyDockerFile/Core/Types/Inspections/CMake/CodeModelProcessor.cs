

using System.Linq;
using System.Text;
using System.Text.Json;
using Spectre.Console;
using static Global.Logging;

namespace EasyDockerFile.Core.Types.Inspections.CMake;

using static CodeModelTypes;

public class CodeModelProcessor 
{
    public static List<CodeModelReplyTarget.Root> ProcessCodeModel(CodeModelRoot root) 
    {   
        if (root.Configurations == null) {
            WriteErrorMessage(
                message: "The root of the CodeModel reply contains no configurations.",
                exitCode: 1,
                exit: true
            );
        }

        if (root.Paths == null) {
            WriteErrorMessage(
                message: "The root of the CodeModel reply contains no paths, unable to process any further.", 
                exitCode: 1, 
                exit: true
            );
        }

        var buildDir = root.Paths.Build;

        if (buildDir == null) {
            WriteErrorMessage(
                message: "The paths object in the CodeModel reply contains no Build directory, unable to process any further.", 
                exitCode: 1, 
                exit: true
            );
        }


        var sourceDir = root.Paths.Source;

        if (buildDir == null) {
            WriteErrorMessage(
                message: "The paths object in the CodeModel reply contains no Source directory, unable to process any further.", 
                exitCode: 1, 
                exit: true
            );
        }


        var cmakeReplyDirectory = Path.Combine(buildDir, ".cmake", "api", "v1", "reply");

        var projectTargets = new List<CodeModelReplyTarget.Root>();
        
        // Due to the possibility of config.Name being null/empty:
        // False -> An alternative name of "Primary" will be provided (if the config.Name is null/empty). 
        // True -> An alternative name of $"Alternative #{alternativeConfigCount}" will be provided (if the config.Name is null/empty). 
        var primaryConfigNameUsed = false;
        var alternativeConfigCount = 0;
        
        foreach (var config in root.Configurations) 
        {
            
            // Initializing a tempConfig is required with the current implementation.
            // This is due to nature of GC in C#, this is required to avoid CS1657.
            // CS1657 occurs when the compile time value for a reference parameter, is itself a reference to another object.
            // This is a horrible design flaw, and needs to be addressed ASAP.
            // TODO: Rewrite this without being an idiot! (Difficulty Level: IMPOSSIBLE CLEARLY)
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/ref-modifiers-errors#writable-reference-variables-require-a-writable-referent
            
            if (string.IsNullOrEmpty(config.Name)) {
                var tempConfig = config;
                AssignMissingConfigName(ref tempConfig, ref primaryConfigNameUsed, ref alternativeConfigCount);
                config.Name = tempConfig.Name; 
            }

            if (config.Projects.Count == 0) {
                WriteErrorMessage(
                    $"Unable to parse the configuration '{config.Name}' as it contains no target projects.", 
                    exitCode: 1, 
                    exit: true
                );
            }

            WriteInformation($"Parsing configuration: {config.Name}\n");


            var builder = new StringBuilder();
            builder.AppendLine();
            

            builder.AppendLine("----------------------------------------------------------------");
            builder.AppendLine();
            
            int targetsParsed = 0;
            foreach (var target in config.Targets) 
            {
                if (target.DirectoryIndex == null || target.ProjectIndex == null || target.JsonFile == null) {
                    WriteWarningMessage($"Unable to process project target: {target.Name} due to missing metadata.");
                    continue;
                }

                if (target.ProjectIndex.Value < 0 || target.ProjectIndex.Value >= config.Projects.Count) {
                    WriteWarningMessage($"Unable to process project target: {target.Name} due to invalid project index.");
                    continue;
                }

                if (target.DirectoryIndex.Value < 0 || target.DirectoryIndex.Value >= config.Directories.Count) {
                    WriteWarningMessage($"Unable to process project target: {target.Name} due to invalid directory index.");
                    continue;
                }

                var project = config.Projects[target.ProjectIndex.Value];
                var directory = config.Directories[target.DirectoryIndex.Value];

                var targetInfoPath = Path.Combine(cmakeReplyDirectory, target.JsonFile);

                if (!File.Exists(targetInfoPath)) {
                    WriteWarningMessage($"Unable to process project target: {target.Name}");
                    WriteErrorMessage($"Unable to locate target information at: {targetInfoPath}");
                    continue;
                }



                string? jsonContent;
                try {
                    jsonContent = File.ReadAllText(targetInfoPath);
                }

                catch (Exception ex) {
                    WriteWarningMessage($"Unable to process project target: {target.Name}");
                    WriteErrorMessage($"{ex.Message}");
                    continue;
                }

                var targetReplyInfo = JsonSerializer.Deserialize<CodeModelReplyTarget.Root>(jsonContent);

                if (targetReplyInfo == null) {
                    WriteWarningMessage($"Unable to process project target: {target.Name}");
                    WriteErrorMessage($"Unable to deserialize target information from: {targetInfoPath}");
                    continue;
                }

                targetReplyInfo.ProjectName = project.Name;
                targetReplyInfo.SourceDirectory = directory.Source;

                targetsParsed++;

                builder.AppendLine($"    Target {targetsParsed}: {target.Name}");
                builder.AppendLine($"    Target ID: {target.Id}");
                builder.AppendLine();

                var sourceNumber = 0;

                foreach (var source in targetReplyInfo.Sources) 
                {
                    // Ensuring the current source group has a CompileGroupIndex
                    if (source.CompileGroupIndex.HasValue) 
                    {

                        var group = targetReplyInfo.CompileGroups[source.CompileGroupIndex.Value];

                        var systemDependencyIncludes = group.Includes.Where(include => include.IsSystem);

                        

                        sourceNumber++;

                        builder.AppendLine($"    Source {sourceNumber}:");
                        builder.AppendLine($"        Path: {source.Path}");
                        builder.AppendLine($"        Language: {group.Language}");
                        builder.Append("        Defines: ");
                        builder.AppendJoin(", ", group.Defines.Select(d => d.Definition));
                        
                        // If the source Target requires external dependencies to be installed.
                        if (systemDependencyIncludes.Any()) {
                            
                            builder.AppendLine();
                            builder.Append("        Includes: ");
                            builder.AppendJoin(", ", systemDependencyIncludes.Select(i => i.Path));
                            builder.AppendLine();
                            builder.AppendLine();
                        }
                    }
                }

                builder.AppendLine("----------------------------------------------------------------");
                builder.AppendLine();

                projectTargets.Add(targetReplyInfo);
            }


            WriteInformation(
                whiteText: "", 
                coloredText: builder.ToString(),
                textColor: "purple",
                tagName: "[[TARGET]]:",
                tagNameColor: "orange"
            );
        }

        return projectTargets;
    }

    private static void AssignMissingConfigName(ref CodeModelConfiguration config, ref bool primaryConfigNameUsed, ref int alternativeConfigCount)
    {
        if (!primaryConfigNameUsed) {
            config.Name = "Primary";
            primaryConfigNameUsed = true;
            return;
        }

        alternativeConfigCount++;
        config.Name = $"Alternative #{alternativeConfigCount}";
    }
}