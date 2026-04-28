using EasyDockerFile.Core.Extensions;
using Global.Build;

namespace EasyDockerFile.Core.API.RepoParser;

// Call MainMenuCommand.GetBuildSystemInfo in ExecuteAsync instead;
[Obsolete(
    "BuildSystems.xml includes this data, this is effectively for reference.\n" +
    "Call MainMenuCommand.GetBuildSystemInfo in ExecuteAsync instead"
)]
public static class BuildSystemLocator 
{
    private readonly static string[] AutotoolsPatterns = ["*configure", "*configure.ac", "*makefile.am"];
    private readonly static string[] BazelPatterns = ["WORKSPACE", "BUILD", "BUILD.bazel"];
    private readonly static string[] CMakeFilePatterns = ["CMakeList.txt", "*CMakePresets.json", "*.cmake", "*toolchain*.cmake"];
    private readonly static string[] MakeFilePatterns = ["Makefile", "makefile", "*GNUmakefile"];
    private readonly static string[] MesonFilePatterns = ["meson.build", "*meson_options.txt"];
    private readonly static string[] MSBuildFilePatterns = [".sln", "*.vcxproj"];
    private readonly static string[] NinjaFilePatterns = ["*build.ninja", "*.ninja_deps"]; 
    public static BuildSystemName? Locate(IEnumerable<string> files) 
    {
        // Checking for a CMakeLists.txt file in the root of the project before other processing.
        // CMake takes priority over other build systems due to it's gold standard status within the C/C++ communities.

        var buildSystemMappings = new Dictionary<string[], BuildSystemName>() {
            { CMakeFilePatterns,   BuildSystemName.CMake     },
            { AutotoolsPatterns,   BuildSystemName.Autotools },
            { BazelPatterns,       BuildSystemName.Bazel     },
            { MakeFilePatterns,    BuildSystemName.Make      },
            { MesonFilePatterns,   BuildSystemName.Meson     },
            { MSBuildFilePatterns, BuildSystemName.MSBuild   },
            { NinjaFilePatterns,   BuildSystemName.Ninja     },
        };

        foreach (var buildSystemMapping in buildSystemMappings) 
        {
            if (files.AnyAreFound(buildSystemMapping.Key)) {
                return buildSystemMapping.Value;
            }
        }


        return null;
    }

    
}