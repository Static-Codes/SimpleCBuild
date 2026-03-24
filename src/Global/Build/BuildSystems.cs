using System.Xml.Linq;
using System.Xml.Serialization;

namespace Global.Build;

[XmlRoot("systems")]
public class BuildSystems
{
    [XmlElement("system")]
    public BuildSystemInfo[] Systems { get; set; } = [];

    public static BuildSystems FromXElement(XElement root)
    {
        return new BuildSystems
        {
            Systems = [.. root.Elements("system").Select(BuildSystemInfo.FromXElement)]
        };
    }
}