using EasyDockerFile.Core.Extensions;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace EasyDockerFile.Core.Types.ImageTypes;

[XmlRoot(ElementName = "image")]
public class Image() 
{
    [XmlElement("full_name")]
    public required string? FullName { get; set; }

    [XmlElement("version")]
    public required string Version { get; set; }

    [XmlArray("supported_architectures")]
    [XmlArrayItem("supported_architecture")]
    public string[] ArchitectureStrings { get; set;} = [];

    [XmlIgnore]
    public IEnumerable<Architecture> SupportedArchitectures => ArchitectureStrings.Select(archString => archString.ToArchitecture());
}

