using Global.Build;

namespace EasyDockerFile.Core.API.RepoParser.BuildSystem;

public static class Locator 
{
    public static BuildSystemName? LocateBuildSystem(IEnumerable<string> files) 
    {
        if (files.Contains("meson.build")) {
            return BuildSystemName.Meson;
        }
        return null;
    }
}