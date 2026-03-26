using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using static Global.Logging;

namespace Global.Build;

[XmlRoot(ElementName="installation")]
public class BuildSystemInstallations { 

	[XmlElement(ElementName="debian")] 
	public DebianBuildSystemInstallation? Debian { get; set; } = null; 

	/// <summary>
    ///     Helper method to create a BuildFiles object from an XElement object.
    /// </summary>
    /// 
    /// <param name="xElement">
    ///     The XElement object to be used in the creation of the BuildFiles object.
    /// </param>
    public static BuildSystemInstallations FromXElement(XElement xElement, string architectureString)
    {
        if (xElement == null) {
            return new BuildSystemInstallations();
        }

		var debianBlock = xElement.Elements("debian");
		
        return new BuildSystemInstallations
        {
			Debian = new DebianBuildSystemInstallation() {
				InstallationCommands = [.. GetInstallationCommands(debianBlock, architectureString)],
			},
        };
    }
	
	private static IEnumerable<string> GetInstallationCommands(IEnumerable<XElement> block, string architectureString) 
	{
		var architectureBlock = block.Elements(architectureString);

		if (!architectureBlock.Any()) {
			WriteWarningMessage("Invalid architecture provided to GetInstallationCommands()");
			return [];
		}

		return architectureBlock
			.Elements("installation_command")
			.Select(cmd => cmd.Value);
	}

	public override string ToString() 
	{
		var debianCommands = Debian?.InstallationCommands ?? [];
		
		var builder = new StringBuilder();
		foreach (var command in debianCommands){
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

