using Global.Build;
using System.Xml.Linq;
using static Global.Constants;
using static Global.Logging;

namespace EasyDockerFile.Core.Loaders;

public static class BuildSystemLoader
{
    public const string BuildSystemXMLPattern = "EasyDockerFile.Resources.BuildSystems.xml";

    /// <summary>
    /// Loads BuildSystems.xml and returns an array of BuildSystemInfo.
    /// </summary>
    public static BuildSystemInfo[] GetBuildSystems(string architectureString)
    {
        WriteInformation("Loading Build Systems.");

        using var stream = _assembly.GetManifestResourceStream(BuildSystemXMLPattern);

        if (stream == null)
        {
            WriteErrorMessage($"Embedded resource '{BuildSystemXMLPattern}' not found.", exit: true);
            return [];
        }

        var doc = XDocument.Load(stream);
        
        if (doc.Root == null)
        {
            WriteErrorMessage("Root element of BuildSystems.xml is null.", exit: true);
            return [];
        }

        var buildSystemsObj = BuildSystems.FromXElement(doc.Root, architectureString);
        var buildSystems = buildSystemsObj.Systems;

        if (buildSystems == null || buildSystems.Length == 0)
        {
            WriteWarningMessage("No build systems found in BuildSystems.xml.");
            return [];
        }

        WriteSuccessMessage($"Loaded {buildSystems.Length} Build Systems.");
        return buildSystems;
    }
}
