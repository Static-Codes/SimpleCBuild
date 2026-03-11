namespace EasyDockerFile.Core.Types.ToolchainTypes;


public enum PackageManagerName 
{
    Buckaroo = 0, // https://buckaroo.pm/
    Conan = 1,    // https://conan.io/
    VCPkg = 2,    // https://github.com/microsoft/vcpkg
    Apt = 3,      // Used for sys deps
    Dnf = 4       // Used for sys deps
}