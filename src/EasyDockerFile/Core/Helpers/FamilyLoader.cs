using EasyDockerFile.Core.Types.ImageTypes;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using static EasyDockerFile.Core.Common.Constants;

namespace EasyDockerFile.Core.Helpers;

public static class FamilyLoader 
{
    private static readonly string ImageXMLPattern = "EasyDockerFile.Resources.Images.xml";

    // Ensures all types that touch ImageFamilies are preserved, even if they arent explicitly used.
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ImageFamilies))]
    // [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "Roots.xml handles the preservation of these types.")]
    // [UnconditionalSuppressMessage("AotAnalysis", "IL3050", Justification = "Due to the analysis above, currently, this method is AOT safe.")]
    public static ImageFamily[] GetFamilies()
    {
        Console.WriteLine("[INFO]: Loading Base Docker images.");

        using var stream = _assembly.GetManifestResourceStream(ImageXMLPattern);

        if (stream == null)
        {
            Console.WriteLine($"[ERROR]: Resource {ImageXMLPattern} not found.");
            Environment.Exit(1);
        }

        var doc = XDocument.Load(stream);
        var familiesObj = FedoraXmlMapper.MapImageFamilies(doc);

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