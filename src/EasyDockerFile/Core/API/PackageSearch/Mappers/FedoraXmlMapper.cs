using System.Xml.Linq;
using EasyDockerFile.Core.API.PackageSearch.Manifests;
using EasyDockerFile.Core.Types.ImageTypes;

// Due to NativeAOT not playing well with System.Xml.Serialization.XmlSerializer
// This class is used to map each node to an XElement which is NativeAOT compatible.
public static class FedoraXmlMapper
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
    public static Image MapImage(XElement el)
    {
        return new Image
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
    
    public static RepoMD MapRepoMD(XDocument doc)
    {
        var root = doc.Root;
        return new RepoMD
        {
            Revision = (long?)root?.Element(FedoraNamespaces.Repo + "revision") ?? 0,
            Data = root?.Elements(FedoraNamespaces.Repo + "data").Select(d => new RepoMDData
            {
                Type = (string?)d.Attribute("type"),
                Checksum = new RepoMDChecksum { 
                    Type = (string?)d.Element(FedoraNamespaces.Repo + "checksum")?.Attribute("type"), 
                    Text = (string?)d.Element(FedoraNamespaces.Repo + "checksum") 
                },
                OpenChecksum = new RepoMDOpenChecksum { 
                    Type = (string?)d.Element(FedoraNamespaces.Repo + "open-checksum")?.Attribute("type"), 
                    Text = (string?)d.Element(FedoraNamespaces.Repo + "open-checksum") 
                },
                Location = new RepoMDLocation { Href = (string?)d.Element(FedoraNamespaces.Repo + "location")?.Attribute("href") },
                Timestamp = (long?)d.Element(FedoraNamespaces.Repo + "timestamp") ?? 0,
                Size = (long?)d.Element(FedoraNamespaces.Repo + "size") ?? 0,
                OpenSize = (long?)d.Element(FedoraNamespaces.Repo + "open-size") ?? 0,
                DatabaseVersion = (int?)d.Element(FedoraNamespaces.Repo + "database_version") ?? 0
            }).ToArray()
        };
    }

    public static FedoraPackage MapPackage(XElement el)
    {
        var format = el.Element(FedoraNamespaces.Common + "format");
        return new FedoraPackage
        {
            Type = (string?)el.Attribute("type"),
            Name = (string?)el.Element(FedoraNamespaces.Common + "name"),
            Arch = (string?)el.Element(FedoraNamespaces.Common + "arch"),
            Version = MapVersion(el.Element(FedoraNamespaces.Common + "version")),
            Checksum = MapPkgChecksum(el.Element(FedoraNamespaces.Common + "checksum")),
            Summary = (string?)el.Element(FedoraNamespaces.Common + "summary"),
            Description = (string?)el.Element(FedoraNamespaces.Common + "description"),
            Packager = (string?)el.Element(FedoraNamespaces.Common + "packager"),
            Url = (string?)el.Element(FedoraNamespaces.Common + "url"),
            Time = MapTime(el.Element(FedoraNamespaces.Common + "time")),
            Size = MapSize(el.Element(FedoraNamespaces.Common + "size")),
            Location = new FedoraPackageLocation { Href = (string?)el.Element(FedoraNamespaces.Common + "location")?.Attribute("href") },
            Format = MapFormat(format)
        };
    }

    private static FedoraPackageVersion? MapVersion(XElement? el) => el == null ? null : new FedoraPackageVersion {
        Epoch = (int?)el.Attribute("epoch") ?? 0,
        Ver = (string?)el.Attribute("ver"),
        Rel = (string?)el.Attribute("rel")
    };

    private static FedoraPackageChecksum? MapPkgChecksum(XElement? el) => el == null ? null : new FedoraPackageChecksum {
        Type = (string?)el.Attribute("type"),
        Pkgid = (string?)el.Attribute("pkgid"),
        Text = (string?)el
    };

    private static FedoraPackageTime? MapTime(XElement? el) => el == null ? null : new FedoraPackageTime {
        File = (long?)el.Attribute("file") ?? 0,
        Build = (long?)el.Attribute("build") ?? 0
    };

    private static FedoraPackageSize? MapSize(XElement? el) => el == null ? null : new FedoraPackageSize {
        Package = (long?)el.Attribute("package") ?? 0,
        Installed = (long?)el.Attribute("installed") ?? 0,
        Archive = (long?)el.Attribute("archive") ?? 0
    };

    private static FedoraPackageFormat? MapFormat(XElement? el) => el == null ? null : new FedoraPackageFormat {
        License = (string?)el.Element(FedoraNamespaces.Rpm + "license"),
        Vendor = (string?)el.Element(FedoraNamespaces.Rpm + "vendor"),
        Group = (string?)el.Element(FedoraNamespaces.Rpm + "group"),
        Buildhost = (string?)el.Element(FedoraNamespaces.Rpm + "buildhost"),
        Sourcerpm = (string?)el.Element(FedoraNamespaces.Rpm + "sourcerpm"),
        Headerrange = new FedoraPackageHeaderRange {
            Start = (int?)el.Element(FedoraNamespaces.Rpm + "header-range")?.Attribute("start") ?? 0,
            End = (int?)el.Element(FedoraNamespaces.Rpm + "header-range")?.Attribute("end") ?? 0
        },
        Provides = new FedoraPackageProvides { Entries = MapEntries(el.Element(FedoraNamespaces.Rpm + "provides")) },
        Requires = new FedoraPackageRequires { Entries = MapEntries(el.Element(FedoraNamespaces.Rpm + "requires")) },
        File = [.. el.Elements(FedoraNamespaces.Common + "file").Select(f => (string)f)]
    };

    private static List<FedoraPackageEntry> MapEntries(XElement? el) {
        if (el == null) return [];
        return [.. el.Elements(FedoraNamespaces.Rpm + "entry").Select(e => new FedoraPackageEntry {
            Name = (string?)e.Attribute("name"),
            Flags = (string?)e.Attribute("flags"),
            Epoch = (int?)e.Attribute("epoch") ?? 0,
            Ver = (string?)e.Attribute("ver"),
            Rel = (string?)e.Attribute("rel")
        })];
    }
}