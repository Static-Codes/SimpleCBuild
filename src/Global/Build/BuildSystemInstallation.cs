using System.Xml.Linq;
using System.Xml.Serialization;

namespace Global.Build;

[XmlRoot(ElementName="installation")]
public class BuildSystemInstallation { 

	[XmlElement(ElementName="debian")] 
	public DebianBuildSystemInstallation? Debian { get; set; } = null; 

	[XmlElement(ElementName="fedora")] 
	public FedoraBuildSystemInstallation? Fedora { get; set; } = null;

	/// <summary>
    ///     Helper method to create a BuildFiles object from an XElement object.
    /// </summary>
    /// 
    /// <param name="xElement">
    ///     The XElement object to be used in the creation of the BuildFiles object.
    /// </param>
    public static BuildSystemInstallation FromXElement(XElement xElement)
    {
        if (xElement == null) {
            return new BuildSystemInstallation();
        }

		var debianBlock = xElement.Elements("debian");
		var fedoraBlock = xElement.Elements("fedora");
		
        return new BuildSystemInstallation
        {
			Debian = new DebianBuildSystemInstallation() {
				InstallationCommands = [.. GetCommands(debianBlock)],
			},
            Fedora = new FedoraBuildSystemInstallation() {
				InstallationCommands = [.. GetCommands(fedoraBlock)],
			}
        };
    }
	
	private static IEnumerable<string> GetCommands(IEnumerable<XElement> block) 
	{
		return block
			.Elements("installation_command")
			.Select(cmd => cmd.Value);
	}
}


[XmlRoot(ElementName="debian")]
public class DebianBuildSystemInstallation { 

	[XmlElement(ElementName="installation_command")] 
	public List<string> InstallationCommands { get; set; } = [];
}

[XmlRoot(ElementName="fedora")]
public class FedoraBuildSystemInstallation { 

	[XmlElement(ElementName="installation_command")] 
	public List<string> InstallationCommands { get; set; } = [];
}


