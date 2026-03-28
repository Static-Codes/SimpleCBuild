namespace EasyDockerFile.Core.Types.Conversion;


public class AutotoolsResources 
{
    // These two members are declared as nullable but, they will never be null at initialization.
    // When one of these temporary files is deleted, the member is marked as null.
    // The app will differentiate between a used and unused file, without additional boolean(s) by performing a simple null check.
    public required string? Auto2CMakePath { get; set; }
    public required string? CMakeInspectPath { get; set; }
}