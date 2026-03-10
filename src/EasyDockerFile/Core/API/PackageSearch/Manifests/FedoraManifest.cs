using System.Xml.Linq;
using System.Text;

namespace EasyDockerFile.Core.API.PackageSearch.Manifests;

public static class FedoraNamespaces 
{
    public static readonly XNamespace Repo = "http://linux.duke.edu/metadata/repo";
    public static readonly XNamespace Common = "http://linux.duke.edu/metadata/common";
    public static readonly XNamespace Rpm = "http://linux.duke.edu/metadata/rpm";
}

// ==========================================
// POCO DATA MODELS (Attributes Removed)
// ==========================================

public class RepoMD 
{ 
    public long Revision; 
    public RepoMDData[]? Data; 
}

public class RepoMDData 
{ 
    public string? Type; 
    public RepoMDChecksum? Checksum; 
    public RepoMDOpenChecksum? OpenChecksum; 
    public RepoMDLocation? Location; 
    public long Timestamp; 
    public long Size; 
    public long OpenSize; 
    public int DatabaseVersion;
}

public class RepoMDChecksum { public string? Type; public string? Text; }
public class RepoMDOpenChecksum { public string? Type; public string? Text; }
public class RepoMDLocation { public string? Href; }

public class FedoraManifest 
{ 
    public FedoraPackage[]? Package; 
    public string? Xmlns; 
    public string? Rpm; 
    public int Packages; 
}

public class FedoraPackage 
{ 
    public string? Type; 
    public string? Name; 
    public string? Arch; 
    public FedoraPackageVersion? Version; 
    public FedoraPackageChecksum? Checksum; 
    public string? Summary; 
    public string? Description; 
    public string? Packager; 
    public string? Url; 
    public FedoraPackageTime? Time; 
    public FedoraPackageSize? Size; 
    public FedoraPackageLocation? Location; 
    public FedoraPackageFormat? Format; 

    // Keep your ToString() logic here if needed
}

public class FedoraPackageVersion { public int Epoch; public string? Ver; public string? Rel; }
public class FedoraPackageChecksum { public string? Type; public string? Pkgid; public string? Text; }
public class FedoraPackageTime { public long File; public long Build; }
public class FedoraPackageSize { public long Package; public long Installed; public long Archive; }
public class FedoraPackageLocation { public string? Href; }

public class FedoraPackageFormat 
{ 
    public string? License; 
    public string? Vendor; 
    public string? Group; 
    public string? Buildhost; 
    public string? Sourcerpm; 
    public FedoraPackageHeaderRange? Headerrange; 
    public FedoraPackageProvides? Provides; 
    public FedoraPackageRequires? Requires; 
    public List<string>? File;
}

public class FedoraPackageHeaderRange { public int Start; public int End; }
public class FedoraPackageProvides { public List<FedoraPackageEntry> Entries = []; }
public class FedoraPackageRequires { public List<FedoraPackageEntry> Entries = []; }
public class FedoraPackageEntry { public string? Name; public string? Flags; public int Epoch; public string? Ver; public string? Rel; }
