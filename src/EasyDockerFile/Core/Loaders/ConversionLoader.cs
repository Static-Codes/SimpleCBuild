using EasyDockerFile.Core.Types.Conversion;
using static Global.Logging;
using static EasyDockerFile.Core.Helpers.ResourceHelper;

namespace EasyDockerFile.Core.Loaders;

public class ConversionLoader 
{
    public static AutotoolsResources LoadAutotoolsResources() 
    {
        string Auto2CMakeResourcePath = "EasyDockerFile.Resources.Utilities.auto2cmake.py";
        string CMakeInspectResourcePath = "EasyDockerFile.Resources.Utilities.cmake_inspect.py";
        
        WriteInformation("Loading Autotools conversion resources.");

        var auto2CMakePath = WriteEmbeddedResourceToDisk(Auto2CMakeResourcePath, isExecutable: true, isPythonFile: true);
        WriteSuccessMessage("Loaded auto2cmake.py");

        var cmakeInspectPath = WriteEmbeddedResourceToDisk(CMakeInspectResourcePath, isExecutable: true, isPythonFile: true);
        WriteSuccessMessage($"Loaded cmake_inspect.py");

        return new() {
            Auto2CMakePath = auto2CMakePath, 
            CMakeInspectPath = cmakeInspectPath
        };
    }

    
}