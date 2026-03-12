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
}

// [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
public class MesonProjectObject 
{
    public string? ProjectName;
    public string? ProjectLanguage;
    public string? ProjectVersion;
    public string? MesonVersion;
    public string? DefaultOptions;

    public override string ToString()
    {
        var fields = GetType().GetFields(_publicInstanceFlag);
        var fieldStrings = fields.Select(field => $"{field.Name}: {field.GetValue(this)}");
        return string.Join(NLC, fieldStrings);
    }

}

public class Functions 
{

    public static void ParseMesonBuildFile(string mesonBuildFile) 
    {
        var fileContents = GetMesonProjectContents(mesonBuildFile);
        
        if (fileContents == null) 
        {
            Console.WriteLine($"[WARNING]: Unable to parse the contents of '{mesonBuildFile}'");
            Console.WriteLine($"[ERROR]: fileContent is null in ParseMesonBuildFile()");
            Environment.Exit(1);
        }

        var projectObject = GetMesonProjectObject(fileContents);
        Console.WriteLine(projectObject);
    }

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

    public static MesonProjectObject GetMesonProjectObject(string mesonBuildFileContents) 
    {
        var match = MesonRegex.MesonProjectObjectRegex.Match(mesonBuildFileContents);

        // Only 5 elements are captured but the count is 6, ensure this is fixed
        // Keys:
        // 0
        // project_name
        // project_language
        // project_version
        // meson_version
        // build_arguments

        if (match.Groups.Count != 6) {
            Console.WriteLine($"[WARNING]: Unable to parse the project object from the contents of the provided meson.build file");
            Console.WriteLine($"[ERROR]: captureGroups.Count does not have a length of 6");
            Environment.Exit(1);
        }

        var projectObject = new MesonProjectObject
        {
            ProjectName = match.Groups["project_name"].Value,
            ProjectLanguage = match.Groups["project_language"].Value,
            ProjectVersion = match.Groups["project_version"].Value,
            MesonVersion = match.Groups["meson_version"].Value,
            DefaultOptions = match.Groups["build_arguments"].Value
        };

        return projectObject;


    }
}