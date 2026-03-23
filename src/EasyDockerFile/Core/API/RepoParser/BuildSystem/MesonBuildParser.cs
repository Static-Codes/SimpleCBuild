using EasyDockerFile.Core.Common;
using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Types.Build.Meson;
using System.Text.RegularExpressions;
using static EasyDockerFile.Core.Types.Build.Meson.MesonRegex;
using static Global.Logging;

namespace EasyDockerFile.Core.API.RepoParser.BuildSystem;

public partial class MesonBuildParser 
{
    private static string[] ExtractArgumentValues(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) {
            return [];
        }

        // Removes the surrounding brackets "[...]"
        string content = input.Trim().TrimStart('[').TrimEnd(']');

        // Seperates each argument found into a unique and sanitized element.
        var matches = GetArrayValueRegex.Matches(content);

        // Returns an array of flags (group=value)
        return [.. matches.Cast<Match>().Select(m => m.Groups["val"].Value)];
    }
    

    private static string? GetMesonProjectContents(string mesonBuildFile) 
    {
        string? fileContents = null;
        
        try {
            fileContents = File.ReadAllText(mesonBuildFile);
        }
        
        catch (Exception ex) {
            WriteWarningMessage($"Unable to parse the contents of '{mesonBuildFile}'");
            Console.WriteLine($"[ERROR]: {ex.GetType()}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
            Environment.Exit(1);
        }

        return fileContents
               .Replace("\u00A0", " ")  // Replaces Non-breaking space
               .Replace("\uFEFF", "") // Removes Byte Order Mark;
               .Replace("\r\n", "\n"); // Sanitizing windows line endings
    }

    private static MesonProjectBlock GetMesonProjectBlock(string mesonBuildFileContents) 
    {
        var match = MesonProjectObjectRegex.Match(mesonBuildFileContents);
        
        if (!match.Success) {
            WriteWarningMessage($"Unable to parse the project object from the meson.build file");
            Environment.Exit(1);
        }

        var block = new MesonProjectBlock
        {
            ProjectName = match.Groups["name"].Value,
            ProjectVersion = match.Groups["project_version"].Value,
            MesonVersion = match.Groups["meson_version"].Value,
            ProjectLanguages = HandleValueOrArray(match, "lang_single", "lang_array", "project language(s)"),
            ProjectLicenses = HandleValueOrArray(match, "license_single", "license_array", "project license(s)")
        };

        if (block.ProjectLanguages.Length == 1 && block.ProjectLanguages[0].Equals("Not Found")) {
            throw new ArgumentException("Languages were not found in the meson project block.");
        }

        block.DefaultOptions = ExtractArgumentValues(match.Groups["build_arguments"].Value);

        return block;
    }

    private static List<MesonDependency> GetMesonProjectDependencies(string mesonFileContent) 
    {
        var matches = MesonDependencyRegex.Matches(mesonFileContent);
        
        if (matches.Count == 0) {
            WriteWarningMessage($"Unable to parse any project dependencies from the provided meson.build file");
            WriteErrorMessage("matches.Count is 0 in GetMesonProjectObject()");
            return [];
        }

        var dependencies = new List<MesonDependency>();

        foreach (Match match in matches) 
        {
            if (match.Success)
            {

                string varName = match.Groups["var"].Value;
                string libName = match.Groups["lib"].Value;
                string systemName = match.Groups["system"].Value;
                string[] fallbackNames = [];

                var fallbackMatch = MesonFallbackRegex.Match(match.Groups["args"].Value);
                if (fallbackMatch.Success) { 
                    fallbackNames = ExtractArgumentValues(fallbackMatch.Groups["fallbacks"].Value);
                }

                dependencies.Add(
                    new MesonDependency() {
                        VariableName = varName,
                        LibraryName = libName,  
                        SystemName = systemName,
                        FallbackLibraries = fallbackNames
                    }
                );
            }

        
        }

        return dependencies;
    }

    private static string[] HandleValueOrArray(Match match, string singleKey, string arrayKey, string name) 
    {
        var single = match.Groups[singleKey];
        var array = match.Groups[arrayKey];

        if (single.Success) {
            return [ single.Value.Trim('\'', '\"', ' ') ];
        }
        
        if (array.Success) {
            return ExtractArgumentValues(array.Value);
        }

        WriteWarningMessage($"Unable to locate {name} in meson.build");
        return ["Not Found"];
    }

    public static void ParseBuildFile(string mesonBuildFile) 
    {
        var fileContents = GetMesonProjectContents(mesonBuildFile);
        
        if (fileContents == null) 
        {
            WriteWarningMessage($"Unable to parse the contents of '{mesonBuildFile}'");
            Console.WriteLine($"[ERROR]: fileContent is null in ParseMesonBuildFile()");
            Environment.Exit(1);
        }

        var projectBlock = GetMesonProjectBlock(fileContents);
        Console.WriteLine(projectBlock);
        var projectDependencies = GetMesonProjectDependencies(fileContents);

        List<MesonDependency> windowsDependencies = [];
        List<MesonDependency> unixDependencies = [];

        foreach (var dependency in projectDependencies) 
        {
            if (dependency.SystemName == null || dependency.SystemName.IsUnix()) {
                unixDependencies.Add(dependency);
                continue;
            }
            if (dependency.SystemName == "windows") {
                windowsDependencies.Add(dependency);
                continue;
            }
            unixDependencies.Add(dependency);
        }

        var dependencies = Platform.IsWindows switch {
            true => windowsDependencies,
            false => unixDependencies,
        };

        Console.WriteLine($"{dependencies.Count} dependencies located.");
        foreach (var dependency in dependencies) {
            Console.WriteLine($"\t- {dependency.LibraryName}");
        }

    }
}