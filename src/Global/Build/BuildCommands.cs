using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Global.Build;

[XmlRoot(ElementName="installation")]
public class BuildSystemCommands 
{ 

	[XmlElement(ElementName="debian")] 
	public DebianBuildSystemCommand? Debian { get; set; } = null;

	/// <summary>
    ///     Helper method to create a BuildFiles object from an XElement object.
    /// </summary>
    /// 
    /// <param name="xElement">
    ///     The XElement object to be used in the creation of the BuildFiles object.
    /// </param>
    public static BuildSystemCommands FromXElement(XElement xElement)
    {
        if (xElement == null) {
            return new BuildSystemCommands();
        }

		var debianBlock = xElement.Elements("debian");
		
        return new BuildSystemCommands
        {
			Debian = new DebianBuildSystemCommand() {
			    BuildCommands = [.. GetCommandsFromBlock(debianBlock)],
			}
        };
    }
	
	private static IEnumerable<string> GetCommandsFromBlock(IEnumerable<XElement> block) 
	{
		return block
			.Elements("build_command")
			.Select(cmd => cmd.Value);
	}

	public override string ToString() 
	{
		var debianCommands = Debian?.BuildCommands ?? [];
		
		var builder = new StringBuilder();
		foreach (var command in debianCommands){
			builder.AppendLine(command);	
		}

		return builder.ToString();
	}
}


[XmlRoot(ElementName="debian")]
public class DebianBuildSystemCommand { 

	[XmlElement(ElementName="build_command")] 
	public List<string> BuildCommands { get; set; } = [];
}

