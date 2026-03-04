using System.Xml.Serialization;

namespace EasyDockerFile.Core.Types.ImageTypes;

[XmlRoot("family")]
public class ImageFamily
{
    [XmlText]
    public string Name { get; set; } = string.Empty;

    [XmlElement("image")]
    public Image[] Images { get; set; } = [];
}
