using System.Xml.Serialization;

namespace EasyDockerFile.Core.Types.ImageTypes;


[XmlRoot("families")]
public class ImageFamilies 
{
    [XmlElement("family")]
    public ImageFamily[] SupportedFamilies { get; set; } = [];
}