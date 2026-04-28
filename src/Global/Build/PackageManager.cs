namespace Global.Build;


public enum PackageManagerName 
{
    Conan = 1,    // https://conan.io/
    VCPkg = 2,    // https://github.com/microsoft/vcpkg
    Apt = 3,      // Used for sys deps
}