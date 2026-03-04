using System.Xml.Serialization;

namespace EasyDockerFile.Core.Types.ImageTypes;

[XmlRoot(ElementName = "images")]
public class Images() 
{
    [XmlElement("image", typeof(Image))]
    public Image[] SupportedImages { get; set; } = [];
}