using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Global.Build;

public class BuildFiles
{
    [XmlElement("global_build_file")]
    public string[] GlobalBuildFiles { get; set; } = [];

    [XmlElement("root_build_file")]
    public string[] RootBuildFiles { get; set; } = [];

    [XmlElement("root_build_pattern")]
    public string[] RootBuildPatterns { get; set; } = [];

    [XmlElement("global_build_pattern")]
    public string[] GlobalBuildPatterns { get; set; } = [];

    /// <summary> 
    ///     Checks all of the files in the repository against the outlined names and patterns in BuildSystems.xml
    /// </summary>
    /// 
    /// <param name="allFiles">
    ///     [Required] An IEnumerable of the type string containing all of the filenames in the repository.
    /// </param>
    /// 
    /// <param name="rootFiles">
    ///     [Optional] An IEnumerable of the type string containing all of the filenames in the root directory of the repository.
    /// </param>
    public bool IsMatch(IEnumerable<string> allFiles, IEnumerable<string>? rootFiles)
    {
        // Checks for an exact match in the root directory of the repository.
        if (rootFiles != null && RootBuildFiles.Any(f => rootFiles.Contains(f, StringComparer.OrdinalIgnoreCase))) {
            return true;
        }

        // Checks for a match using each of the root build patterns with BuildRegex() 
        if (rootFiles != null && RootBuildPatterns.Length > 0)
        {
            foreach (var pattern in RootBuildPatterns)
            {
                var regex = BuildRegex(pattern);
                if (rootFiles.Any(f => regex.IsMatch(f))) {
                    return true;
                }
            }
        }

        // Checks for an exact filename match at anywhere in the repository.
        if (GlobalBuildFiles.Any(f => allFiles.Any(af => af.EndsWith(f, StringComparison.OrdinalIgnoreCase)))) {
            return true;
        }

        // Checks for a match using each of the global build patterns with BuildRegex()
        if (GlobalBuildPatterns.Length > 0)
        {
            foreach (var pattern in GlobalBuildPatterns)
            {
                var regex = BuildRegex(pattern);
                if (allFiles.Any(f => regex.IsMatch(Path.GetFileName(f)))) {
                    return true;
                }
            }
        }

        return false;
    }

    private static Regex BuildRegex(string pattern)
    {
        return new Regex(
            string.Join("", [
                "^", 
                Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."),
                "$"
            ]),
            RegexOptions.IgnoreCase
        );
    }


    /// <summary>
    ///     Helper method to create a BuildFiles object from an XElement object.
    /// </summary>
    /// 
    /// <param name="xElement">
    ///     The XElement object to be used in the creation of the BuildFiles object.
    /// </param>
    public static BuildFiles FromXElement(XElement xElement)
    {
        if (xElement == null) {
            return new BuildFiles();
        }

        return new BuildFiles
        {
            GlobalBuildFiles = [.. xElement.Elements("global_build_file").Select(x => x.Value)],
            RootBuildFiles = [.. xElement.Elements("root_build_file").Select(x => x.Value)],
            RootBuildPatterns = [.. xElement.Elements("root_build_pattern").Select(x => x.Value)],
            GlobalBuildPatterns = [.. xElement.Elements("global_build_pattern").Select(x => x.Value)]
        };
    }


}
