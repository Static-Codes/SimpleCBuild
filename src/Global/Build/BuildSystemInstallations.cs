using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Global.Build;

[XmlRoot(ElementName="installation")]
public class BuildSystemInstallations { 

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
    public static BuildSystemInstallations FromXElement(XElement xElement)
    {
        if (xElement == null) {
            return new BuildSystemInstallations();
        }

		var debianBlock = xElement.Elements("debian");
		var fedoraBlock = xElement.Elements("fedora");
		
        return new BuildSystemInstallations
        {
			Debian = new DebianBuildSystemInstallation() {
				InstallationCommands = [.. GetInstallationCommands(debianBlock)],
			},
            Fedora = new FedoraBuildSystemInstallation() {
				InstallationCommands = [.. GetInstallationCommands(fedoraBlock)],
			}
        };
    }
	
	private static IEnumerable<string> GetInstallationCommands(IEnumerable<XElement> block) 
	{
		return block
			.Elements("installation_command")
			.Select(cmd => cmd.Value);
	}

	public override string ToString() 
	{
		var debianCommands = Debian?.InstallationCommands ?? [];
		var fedoraCommands = Fedora?.InstallationCommands ?? [];
		
		var builder = new StringBuilder();
		foreach (var command in debianCommands){
			builder.AppendLine(command);	
		}

		foreach (var command in fedoraCommands) {
			builder.AppendLine(command);
		}

		return builder.ToString();
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


