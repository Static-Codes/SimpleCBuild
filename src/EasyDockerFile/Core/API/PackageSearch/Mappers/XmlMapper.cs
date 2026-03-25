using System.Xml.Linq;
using EasyDockerFile.Core.API.PackageSearch.Manifests;
using DockerFileSharp.Common.Image;

namespace EasyDockerFile.Core.API.PackageSearch.Mappers;

// Due to NativeAOT not playing well with System.Xml.Serialization.XmlSerializer
// This class is used to map each node to an XElement which is NativeAOT compatible.
// Update: 
// As of 03/13/2026 EasyDockerFile no longer uses NativeAOT as XElement is still more optimized, so the code will remain.
// As of 03/25/2026 XmlMapper only supports one family (Debian). This is done for simplicity.

public static class XmlMapper
{
    // Mappings for ImageFamilies.cs -> <families>
    public static ImageFamilies MapImageFamilies(XDocument doc)
    {
        var root = doc.Root; // Expected root is <families>
        return new ImageFamilies
        {
            SupportedFamilies = root?.Elements("family")
                                     .Select(MapImageFamily)
                                     .ToArray() ?? []
        };
    }

    // Mappings for ImageFamily.cs -> <family>
    public static ImageFamily MapImageFamily(XElement el)
    {
        return new ImageFamily
        {
            // Name is [XmlText], meaning it's the text node of the <family> element
            Name = el.Nodes().OfType<XText>().FirstOrDefault()?.Value.Trim() ?? string.Empty,
            
            Images = el.Elements("image")
                       .Select(MapImage)
                       .ToArray() ?? []
        };
    }

    // Mappings for Image.cs -> <image>
    public static IsoImage MapImage(XElement el)
    {
        return new IsoImage
        {
            FullName = (string)el.Element("full_name")! ?? string.Empty,
            Version = (string)el.Element("version")! ?? string.Empty,
            
            // Maps <supported_architectures><supported_architecture>x86_64</...></...>
            ArchitectureStrings = el.Element("supported_architectures")?
                                   .Elements("supported_architecture")
                                   .Select(x => x.Value)
                                   .ToArray() ?? []
        };
    }

}