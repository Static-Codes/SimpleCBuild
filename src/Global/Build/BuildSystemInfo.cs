using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Global.Build;

public class BuildSystemInfo
{
    [XmlElement("name")]
    public required string Name { get; set; }

    [XmlElement("build_files")]
    public required BuildFiles BuildFiles { get; set; }
    
    [XmlElement("installation")]
    public required BuildSystemInstallation Installation { get; set; }

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
            Installation = BuildSystemInstallation.FromXElement(xElement.Element("installation")!),
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
    ///     Returns either the found BuildSystemName or a null value
    /// </summary>
    public BuildSystemName? GetBuildSystemName() => Enum.TryParse<BuildSystemName>(Name, true, out var result) ? result : null;

    /// <summary>
    ///     Returns either the found PackageManagerName or a null value
    /// </summary>
    public PackageManagerName? GetPackageManagerName() => Enum.TryParse<PackageManagerName>(Name, true, out var result) ? result : null;
}
