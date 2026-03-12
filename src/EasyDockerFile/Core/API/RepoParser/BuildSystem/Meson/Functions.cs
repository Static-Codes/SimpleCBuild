using System.Text;
using System.Text.RegularExpressions;
using static EasyDockerFile.Core.Common.Constants;

namespace EasyDockerFile.Core.API.RepoParser.BuildSystem.Meson;

public static partial class MesonRegex
{
    [GeneratedRegex(
        pattern:
        @"project\(\s*'(?<project_name>.*)', " +
        @"'(?<project_language>.*)',\s*" +
        @"version : '(?<project_version>.*)',\s*" +
        @"meson_version : '(?<meson_version>.*)',\s*" +
        @"default_options : (?<build_arguments>\['.*'])", 
        options: RegexOptions.IgnoreCase,
        cultureName: "en-US"
    )]
    public static partial Regex MesonProjectObjectRegex { get; }

    [GeneratedRegex(@"'(?<val>.*?)'")]
    public static partial Regex GetArrayValueRegex { get; }

    [GeneratedRegex(@"project\s*\((?<content>.*?)\)", RegexOptions.Singleline)]
    public static partial Regex GetProjectBlock { get; }

    [GeneratedRegex(@"^\s*'(?<name>.*?)'\s*,\s*'(?<lang>.*?)'")]
    public static partial Regex GetProjectNameAndLanguage { get; }
}

public class MesonProjectBlock
{
    public string? ProjectName;
    public string? ProjectLanguage;
    public string? ProjectVersion;
    public string? MesonVersion;
    public string[]? DefaultOptions;

    public override string ToString()
    {
        var fields = GetType().GetFields(_publicInstanceFlag);
        var stringBuilder = new StringBuilder();
        foreach (var field in fields) 
        {
            if (field.FieldType == typeof(string)) {
                stringBuilder.AppendLine($"{field.Name}: {field.GetValue(this)}");
            }

            if (field.FieldType == typeof(string[])) {
                stringBuilder.AppendLine($"{field.Name}");
                var arrayMembersObj = field.GetValue(this);

                ArgumentNullException.ThrowIfNull(arrayMembersObj, nameof(arrayMembersObj));

                var arrayMembers = (string[])arrayMembersObj;

                foreach (string member in arrayMembers) {
                    stringBuilder.AppendLine($"\t- {member}");
                }
            }
        }
        return stringBuilder.ToString();
    }

}

public partial class MesonProjectFunctions 
{
    private static string? GetMesonProjectContents(string mesonBuildFile) 
    {
        string? fileContents = null;
        
        try {
            fileContents = File.ReadAllText(mesonBuildFile);
        }
        
        catch (Exception ex) {
            Console.WriteLine($"[WARNING]: Unable to parse the contents of '{mesonBuildFile}'");
            Console.WriteLine($"[ERROR]: {ex.GetType()}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
            Environment.Exit(1);
        }

        return fileContents;
    }

    public static MesonProjectBlock GetMesonProjectBlock(string mesonBuildFileContents) 
    {
        var match = MesonRegex.GetProjectBlock.Match(mesonBuildFileContents);
        
        if (!match.Success) {
            Console.WriteLine($"[WARNING]: Unable to parse the project object from the contents of the provided meson.build file");
            Console.WriteLine($"[ERROR]: match.Success is false in GetMesonProjectObject");
            Environment.Exit(1);
        }
        
        string innerContent = match.Groups["content"].Value;
        
        var positionalMatch = MesonRegex.GetProjectNameAndLanguage.Match(innerContent);

        var block = new MesonProjectBlock() {
            ProjectName = positionalMatch.Groups["name"].Value,
            ProjectLanguage = positionalMatch.Groups["lang"].Value,
            
            // Retrieves all other string arguments where ordering may differ.
            ProjectVersion = ExtractArgumentValue(innerContent, "version"),
            MesonVersion = ExtractArgumentValue(innerContent, "meson_version")
        };

        var options = ExtractArgumentValue(innerContent, "default_options");

        // Serializes the meson array default_options = ['arg1', 'arg2' ...] to a c# string array
        block.DefaultOptions = ExtractArgumentValues(options);

        return block;
    }

    private static string ExtractArgumentValue(string input, string key)
    {
        // Matches both:
        // key : 'value'
        // key : [value]
        var pattern = $@"{key}\s*:\s*(?<val>'.*?'|\[.*?\])";
        var match = Regex.Match(input, pattern, RegexOptions.Singleline);
        return match?.Groups?["val"].Value?.Trim('\'') ?? ""; // Trims surrounding single quotes.
    }

    private static string[] ExtractArgumentValues(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) {
            return [];
        }

        // Removes the surrounding brackets "[...]"
        string content = input.Trim().TrimStart('[').TrimEnd(']');

        // Seperates each argument found into a unique and sanitized element.
        var matches = MesonRegex.GetArrayValueRegex.Matches(content);

        // Returns an array of flags (group=value)
        return [.. matches.Cast<Match>().Select(m => m.Groups["val"].Value)];
    }

    public static void ParseBuildFile(string mesonBuildFile) 
    {
        var fileContents = GetMesonProjectContents(mesonBuildFile);
        
        if (fileContents == null) 
        {
            Console.WriteLine($"[WARNING]: Unable to parse the contents of '{mesonBuildFile}'");
            Console.WriteLine($"[ERROR]: fileContent is null in ParseMesonBuildFile()");
            Environment.Exit(1);
        }

        var projectBlock = GetMesonProjectBlock(fileContents);
        Console.WriteLine(projectBlock);
    }
}