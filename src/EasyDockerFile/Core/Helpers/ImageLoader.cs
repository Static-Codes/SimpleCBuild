using System.Reflection;
using System.Xml.Serialization;
using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Types.ImageTypes;

namespace EasyDockerFile.Core.Helpers;

public static class ImageLoader 
{
    private static readonly string ImageXMLPattern = "EasyDockerFile.Resources.Images.xml";
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    public static ImageFamily[] GetFamilies()
    {
        Console.WriteLine("[INFO]: Loading Base Docker images.");

        var serializer = new XmlSerializer(typeof(ImageFamilies));

        using var stream = _assembly.GetManifestResourceStream(ImageXMLPattern);

        if (stream == null)
        {
            Console.WriteLine($"[ERROR]: Resource {ImageXMLPattern} not found.");
            Environment.Exit(1);
        }

        var familiesObj = (ImageFamilies)serializer.Deserialize(stream)!;
        var families = familiesObj.SupportedFamilies;

        if (families == null || families.Length == 0)
        {
            Console.WriteLine("[WARNING]: No families found in XML.");
            return [];
        }

        foreach (var family in families)
        {
            family.Name = family.Name.Trim();
            Console.WriteLine($"[INFO]: Loaded base family: {family.Name}");
        }

        Console.WriteLine("[SUCCESS]: Loaded Base Docker families.");
        return families;
    }

}