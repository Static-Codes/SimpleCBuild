using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EasyDockerFile.Core.Types.Build.Base;

public class BuildSystemInfo
{
    [XmlElement("name")]
    public required string Name { get; set; }

    [XmlElement("build_files")]
    public required BuildFiles BuildFiles { get; set; }

    /// <summary>
    ///     Helper method to create a BuildSystemInfo object from an XElement object.
    /// </summary>
    /// 
    /// <param name="xElement">
    ///     The XElement object to be used in the creation of the BuildSystemInfo object.
    /// </param>
    public static BuildSystemInfo FromXElement(XElement xElement)
    {
        return new BuildSystemInfo
        {
            Name = (string)xElement.Element("name")! ?? string.Empty,
            BuildFiles = BuildFiles.FromXElement(xElement.Element("build_files")!),
        };
    }

    /// <summary>
    ///     Checks if this build system is detected in the provided list of filenames.
    /// </summary>
    /// 
    /// <param name="allFiles">
    ///     [Required] An IEnumerable of the type string containing all of the filenames in the repository.
    /// </param>
    /// 
    /// <param name="rootFiles">
    ///     [Optional] An IEnumerable of the type string containing all of the filenames in the root directory of the repository.
    /// </param>
    public bool IsPresent(IEnumerable<string> allFiles, IEnumerable<string>? rootFiles = null) => BuildFiles.IsMatch(allFiles, rootFiles);

    /// <summary>
    ///     Performs a remote check using the provided RepoClient to see if any of the 
    ///     explicitly named build files (root or global) exist in the repository.
    /// </summary>
    /// 
    /// <param name="client">
    ///     The RepoClient to be used in the remote detection.
    /// </param>
    public bool IsDetectedRemote(Git.RepoClient client)
    {
        // Checking for explicit global build files
        foreach (var file in BuildFiles.GlobalBuildFiles)
        {
            if (client.RepoContainsFile(file)) {
                return true;
            }
        }

        // Checking for explicit root build files
        // Currently: 
        // RepoContainsFile uses GitHub search which is global in nature.
        // This needs to be reworked in a later commit.
        foreach (var file in BuildFiles.RootBuildFiles)
        {
            if (client.RepoContainsFile(file)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Returns either the found BuildSystemName or a null value
    /// </summary>
    public BuildSystemName? GetBuildSystemName() => Enum.TryParse<BuildSystemName>(Name, true, out var result) ? result : null;

    /// <summary>
    ///     Returns either the found PackageManagerName or a null value
    /// </summary>
    public PackageManagerName? GetPackageManagerName() => Enum.TryParse<PackageManagerName>(Name, true, out var result) ? result : null;
}
