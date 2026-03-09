using System.Text;
using System.Xml.Serialization;
using static EasyDockerFile.Core.Common.Constants;

namespace EasyDockerFile.Core.API.PackageSearch.Manifests;

// ==========================================
// Start of repomd.xml
// ==========================================

[XmlRoot(ElementName="repomd", Namespace = "http://linux.duke.edu/metadata/repo")]
public class RepoMD 
{ 
    [XmlElement(ElementName="revision")] 
    public long Revision; 

    [XmlElement(ElementName="data")] 
    public RepoMDData[]? Data; 
}

[XmlRoot(ElementName="data", Namespace = "http://linux.duke.edu/metadata/repo")]
public class RepoMDData 
{ 
    [XmlAttribute(AttributeName="type")] 
    public string? Type; 

    [XmlElement(ElementName="checksum")] 
    public RepoMDChecksum? Checksum; 

    [XmlElement(ElementName="open-checksum")] 
    public RepoMDOpenChecksum? OpenChecksum; 
    [XmlElement(ElementName="location")] 
    public RepoMDLocation? Location; 

    [XmlElement(ElementName="timestamp")] 
    public long Timestamp; 

    [XmlElement(ElementName="size")] 
    public long Size; 

    [XmlElement(ElementName="open-size")] 
    public long OpenSize; 

    [XmlElement(ElementName="database_version")]
    public int DatabaseVersion;
}

[XmlRoot(ElementName="checksum", Namespace = "http://linux.duke.edu/metadata/repo")]
public class RepoMDChecksum 
{ 
    [XmlAttribute(AttributeName="type")] 
    public string? Type; 

    [XmlText] 
    public string? Text; 
}

[XmlRoot(ElementName="open-checksum", Namespace = "http://linux.duke.edu/metadata/repo")]
public class RepoMDOpenChecksum { 
    [XmlAttribute(AttributeName="type")] 
    public string? Type; 

    [XmlText] 
    public string? Text; 
}

[XmlRoot(ElementName="location", Namespace = "http://linux.duke.edu/metadata/repo")]
public class RepoMDLocation { 
    [XmlAttribute(AttributeName="href")] 
    public string? Href; 
}


// ==========================================
// SECOND: primary.......xml
// ==========================================

// ==========================================
// End of repomd.xml
// ==========================================


// ==========================================
// Start of primary.xml
// ==========================================

/// <summary>
/// Namespace constants used in Fedora's primary manifest.
/// </summary>
public static class FedoraNamespaces 
{
    public const string Common = "http://linux.duke.edu/metadata/common";
    public const string Rpm = "http://linux.duke.edu/metadata/rpm";
}

[XmlRoot(ElementName="metadata")]
public class FedoraManifest 
{ 
	[XmlElement(ElementName="package")] 
	public FedoraPackage[]? Package; 

	[XmlAttribute(AttributeName="xmlns")] 
	public string? Xmlns; 

	[XmlAttribute(AttributeName="rpm")] 
	public string? Rpm; 

	[XmlAttribute(AttributeName="packages")] 
	public int Packages; 

	[XmlText] 
	public string? Text;
}


[XmlRoot(ElementName="package", Namespace = FedoraNamespaces.Common)]
public class FedoraPackage 
{ 
    [XmlAttribute(AttributeName="type")] 
    public string? Type; 

    [XmlElement(ElementName="name", Namespace = FedoraNamespaces.Common)] 
    public string? Name; 

    [XmlElement(ElementName="arch", Namespace = FedoraNamespaces.Common)] 
    public string? Arch; 

    [XmlElement(ElementName="version", Namespace = FedoraNamespaces.Common)] 
    public FedoraPackageVersion? Version; 

    [XmlElement(ElementName="checksum", Namespace = FedoraNamespaces.Common)] 
    public FedoraPackageChecksum? Checksum; 

    [XmlElement(ElementName="summary", Namespace = FedoraNamespaces.Common)] 
    public string? Summary; 

    [XmlElement(ElementName="description", Namespace = FedoraNamespaces.Common)] 
    public string? Description; 

    [XmlElement(ElementName="packager", Namespace = FedoraNamespaces.Common)] 
    public string? Packager; 

    [XmlElement(ElementName="url", Namespace = FedoraNamespaces.Common)] 
    public string? Url; 

    [XmlElement(ElementName="time", Namespace = FedoraNamespaces.Common)] 
    public FedoraPackageTime? Time; 

    [XmlElement(ElementName="size", Namespace = FedoraNamespaces.Common)] 
    public FedoraPackageSize? Size; 

    [XmlElement(ElementName="location", Namespace = FedoraNamespaces.Common)] 
    public FedoraPackageLocation? Location; 

    [XmlElement(ElementName="format", Namespace = FedoraNamespaces.Common)] 
    public FedoraPackageFormat? Format; 
    
    public override string ToString()
    {
        var properties = typeof(FedoraPackage).GetProperties(_publicFlag);
        var sb = new StringBuilder();

        sb.AppendLine($"--- Package: {Name} ---");

        foreach (var prop in properties)
        {
            var value = prop.GetValue(this);
            
            if (value == null)
            {
                sb.AppendLine($"{prop.Name}: N/A");
                continue;
            }

            //  && !prop.PropertyType.IsEnum
            if (prop.PropertyType.Assembly == typeof(FedoraPackage).Assembly)
            {
                sb.AppendLine($"{prop.Name}:");
                sb.AppendLine(FormatPackage(value, "  "));
            }
            else
            {
                sb.AppendLine($"{prop.Name}: {value}");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Helper method used in the ToString() override for FedoraPackage
    /// </summary>
    private static string FormatPackage(object obj, string indent)
    {
        var builder = new StringBuilder();
        var properties = obj.GetType().GetProperties(_publicFlag);
        
        foreach (var property in properties)
        {
            var value = property.GetValue(obj) ?? "N/A";

            // Handles nested entries of Types that havent defined a dedicated ToString() override.
            if (value is System.Collections.IEnumerable and not string) {
                builder.AppendLine($"{indent}{property.Name} ([IEnumerable]):");
                continue;
            }

            // Handles all other entries
            builder.AppendLine($"{indent}{property.Name}: {value}");
            
        }
        return builder.ToString();
    }
}

public class FedoraPackageVersion 
{ 
    [XmlAttribute(AttributeName="epoch")] 
    public int Epoch; 

    [XmlAttribute(AttributeName="ver")] 
    public string? Ver; 

    [XmlAttribute(AttributeName="rel")] 
    public string? Rel; 
}

public class FedoraPackageChecksum 
{ 
    [XmlAttribute(AttributeName="type")] 
    public string? Type; 

    [XmlAttribute(AttributeName="pkgid")] 
    public string? Pkgid; 

    [XmlText] 
    public string? Text; 
}

public class FedoraPackageTime 
{ 
    [XmlAttribute(AttributeName="file")] 
    public long File; 

    [XmlAttribute(AttributeName="build")] 
    public long Build; 
}

public class FedoraPackageSize 
{ 
    [XmlAttribute(AttributeName="package")] 
    public long Package; 

    [XmlAttribute(AttributeName="installed")] 
    public long Installed; 

    [XmlAttribute(AttributeName="archive")] 
    public long Archive; 
}

public class FedoraPackageLocation 
{ 
    [XmlAttribute(AttributeName="href")] 
    public string? Href; 
}

public class FedoraPackageFormat 
{ 
    [XmlElement(ElementName="license", Namespace = FedoraNamespaces.Rpm)] 
    public string? License; 

    [XmlElement(ElementName="vendor", Namespace = FedoraNamespaces.Rpm)] 
    public string? Vendor; 

    [XmlElement(ElementName="group", Namespace = FedoraNamespaces.Rpm)] 
    public string? Group; 

    [XmlElement(ElementName="buildhost", Namespace = FedoraNamespaces.Rpm)] 
    public string? Buildhost; 

    [XmlElement(ElementName="sourcerpm", Namespace = FedoraNamespaces.Rpm)] 
    public string? Sourcerpm; 

    [XmlElement(ElementName="header-range", Namespace = FedoraNamespaces.Rpm)] 
    public FedoraPackageHeaderRange? Headerrange; 

    [XmlElement(ElementName="provides", Namespace = FedoraNamespaces.Rpm)] 
    public FedoraPackageProvides? Provides; 

    [XmlElement(ElementName="requires", Namespace = FedoraNamespaces.Rpm)] 
    public FedoraPackageRequires? Requires; 

    [XmlElement(ElementName="file", Namespace = FedoraNamespaces.Common)] 
    public List<string>? File;
}

public class FedoraPackageHeaderRange 
{ 
    [XmlAttribute(AttributeName="start")] 
    public int Start; 

    [XmlAttribute(AttributeName="end")] 
    public int End; 
}

public class FedoraPackageProvides 
{ 
    [XmlElement(ElementName = "entry", Namespace = FedoraNamespaces.Rpm)]
    public List<FedoraPackageEntry> Entries = []; 
}

public class FedoraPackageRequires 
{ 
    [XmlElement(ElementName="entry", Namespace = FedoraNamespaces.Rpm)] 
    public List<FedoraPackageEntry> Entries = []; 
}

public class FedoraPackageEntry 
{ 
    [XmlAttribute(AttributeName="name")] 
    public string? Name; 

    [XmlAttribute(AttributeName="flags")] 
    public string? Flags; 

    [XmlAttribute(AttributeName="epoch")] 
    public int Epoch; 

    [XmlAttribute(AttributeName="ver")] 
    public string? Ver; 

    [XmlAttribute(AttributeName="rel")] 
    public string? Rel; 
}

// ==========================================
// End of primary.xml
// ==========================================