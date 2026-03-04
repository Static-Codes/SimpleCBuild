namespace EasyDockerFile.Core.API.PackageSearch;

public class PackageSearchApi
{
    // The platform identifier defined for the provided architecture.
    public virtual string? PlatformIdentifer { get;}
    // The base URI for the search api associated with the given Linux distribution.
    public virtual string? PackageFileUri { get; }

    // These are the names of the packages to include by default, if package resolving fails.
    public virtual string[] FallbackPackages { get; } = [];
}


