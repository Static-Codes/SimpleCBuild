namespace EasyDockerFile.Core.Types.Conversion;



/// <summary>
/// Holds the response data for AutotoolsConverter.ConvertToCMake
/// </summary>
public class AutotoolsConversionResponse 
{
    /// <summary>
    /// If the conversion completed successfully.
    /// </summary>
    public required bool Completed { get; set; }

    /// <summary>
    /// The path to the generated CMakeList.txt file in the project directory's root. <br/>
    /// Note: An error will be thrown trying to access this member, if Completed is false.
    /// </summary>
    public required string RootCMakeListsPath { get; set; }
    
    /// <summary>
    /// The path to the temporary utility file, that will be used to convert the cmake file to a JSON object, then serialized to a custom type later in the session.
    /// </summary>
    public required string CMakeInspectPath { get; set; }
}