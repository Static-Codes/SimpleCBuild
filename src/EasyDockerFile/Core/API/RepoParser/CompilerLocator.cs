using EasyDockerFile.Core.Extensions;
using Global.Build;

namespace EasyDockerFile.Core.API.RepoParser;

// TODO: Implement these checks.
// Additional Checks to resolve X (Build System) to Y (Compiler)
// 1. Autotools/Make (GCC) -> Default for Linux compilation(s) using Makefiles
// 2. Autotools/Make (Intel) -> Check for instances of "CC=icc" and "CXX=icpc" inside both Makefiles and configure scripts
// 3. CMake (MSVC) -> Check for instances of "Visual Studio" in CMakeCache.txt
// 4. CMake (Clang/GCC) -> Check for the absolute paths to either "/usr/bin/gcc" or "/usr/bin/clang"
// 5. VCPkg (All) -> If present, check the "triplets/" directory within the specified project.
      // Windows Specific: -x64-windows and -x86-windows uses MSVC
      // Linux Specific: -x64-linux-clang obviously uses Clang
      // MacOS Specific: N/A not supported by VCPkg to my knowledge.


public class CompilerLocator 
{   
    // Currently CppExtensions is unused, but it will remain here for reference.
    private static readonly string[] CppExtensions = [".cpp", ".cc", ".cxx", ".hpp", ".hh"];
    private static readonly string[] IntelExtensions = [".icc", ".icpc", "icx", "icpx"];

    private static bool IntelCompilerFound(IEnumerable<string> files) {
        return files.Any(f => IntelExtensions.Any(ext => f.EndsWith(ext) || f.Contains(ext)));
    }

    private static bool IsClangProject(IEnumerable<string> files, bool IsWindows) 
    {
        // The presence of "compile_commands.json" is a strong indicator that a Clang-based toolchain is present.
        // Clang-based toolchains include but are not limited to: Bear, Ninja, etc.
        // The presence of the linting tool ".clang-tidy" is another strong indicator that a Clang-based toolchain is present. 
        // The exclusion of ".clang-format" is due to the possibility that it's inclusion is purely stylistic.
            // - As is the case with select GCC/MSVC-based projects.
        
        var clangPossiblyPresent = files.AnyAreFound([".clang-tidy", "compile_commands.json"]);

        // If the user is on Windows, an additional check for "clang-cl" is performed.
        return IsWindows ? clangPossiblyPresent || ClangCLPresent(files) : clangPossiblyPresent;
    }

    private static bool IsIntelProject(IEnumerable<string> files) {
        return (OperatingSystem.IsLinux() || OperatingSystem.IsWindows()) && IntelCompilerFound(files);
    }
    
    private static bool ClangCLPresent(IEnumerable<string> files) => files.Any(f => f.Contains("clang-cl"));

    /// <summary>
    /// Returns Clang if present; otherwise, MSVC is used for Windows, and GCC is used for Linux.
    /// </summary>
    private static CompilerName HandleClangCheck(IEnumerable<string> files) 
    {
        if (OperatingSystem.IsWindows()) {
            return IsClangProject(files, IsWindows: true) ? CompilerName.Clang : CompilerName.MSVC;
        }

        return IsClangProject(files, IsWindows: false) ? CompilerName.Clang : CompilerName.GCC;
    }
    
    /// <summary>
    /// Performs a platform check and project files in an attempt to resolve the compiler in use. </br>
    /// 
    /// Return Values: <br/>
    /// 
    /// Windows -> CompilerName.MSVC or CompilerName.Clang <br/>
    /// MacOS   -> CompilerName.AppleClang <br/>
    /// Linux   -> CompilerName.GCC or CompilerName.Clang
    /// 
    /// 
    /// Special Case: <br/>
    /// CompilerName.Intel is returned for projects that involve 
    /// </summary>
    public static CompilerName LocateFromProject(IEnumerable<string> files) 
    {
        // MacOS almost exclusively uses their own Clang fork.
        // For the purposes of scope, non Apple Clang projects will NOT be supported by SimpleCBuild on macOS.
        if (OperatingSystem.IsMacOS()) {
            return CompilerName.AppleClang;
        }

        // Intel's compiler(s) dont support macOS, so this check can simply remain under the IsMacOS() check.
        if (IsIntelProject(files)) {
            return CompilerName.IntelClassic;
        }

        // Handling Clang or MSVC/GCC (depending on platform).
        return HandleClangCheck(files);
        
    }
}