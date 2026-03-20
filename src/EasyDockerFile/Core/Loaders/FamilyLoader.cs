using EasyDockerFile.Core.API.PackageSearch.Mappers;
using DockerFileSharp.Common.Image;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using static Global.Constants;
using static Global.Logging;

namespace EasyDockerFile.Core.Loaders;

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

        if (stream == null) {
            WriteErrorMessage($"Resource {ImageXMLPattern} not found.", exit: true);
        }

        var doc = XDocument.Load(stream);
        var familiesObj = FedoraXmlMapper.MapImageFamilies(doc);

        var families = familiesObj.SupportedFamilies;

        if (families == null || families.Length == 0)
        {
            WriteWarningMessage("No families found in XML.");
            return [];
        }

        foreach (var family in families)
        {
            family.Name = family.Name.Trim();
            WriteSuccessMessage($"Loaded base family: {family.Name}");
        }

        WriteSuccessMessage("Loaded DockerImage families.");
        return families;
    }
}