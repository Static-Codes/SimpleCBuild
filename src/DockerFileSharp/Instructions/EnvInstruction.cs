using DockerFileSharp.Common;
using System.Text;

namespace DockerFileSharp.Instructions;


/// <summary>
///     Implementation of the ENV Dockerfile Instruction. <br/>
///     The ENV instruction sets the environment variable key(s) to the value(s). <br/><br/>
///     
///     These value(s) will be in the environment for all subsequent instructions in the build stage. <br/>
///     These value(s) can be replaced inline as well. <br/>
///     These value(s) will be interpreted for other environment variables. <br/>
///         - Quote characters will be removed if they are not escaped. <br/>
///         - Like command line parsing, quotes and backslashes can be used to include spaces within values. <br/>
///     For more information, see: https://docs.docker.com/reference/dockerfile/#env
/// </summary>
/// <param name="KeyValuePairs">
///     A Dictionary containing the key and value pairs to be used in the .Build() command.
/// </param>

// See docs/methodology.md for more information about implementating IDockerInstruction.
public record EnvInstruction(Dictionary<string, string> KeyValuePairs) : IDockerInstruction
{
    private string HandleReturnValues() 
    {
        var builder = new StringBuilder();
        builder.Append("ENV ");

        foreach (var pair in KeyValuePairs) 
        {
            // Appending each key-value pair with a trailing space.
            builder.Append($"{pair.Key}=\"{pair.Value}\" ");
        }
        // Trims the last trailing space.
        return builder.ToString().TrimEnd();
    }

    public string Build() {
        return KeyValuePairs.Count switch {
            0 => string.Empty,
            _ => HandleReturnValues(), // 1 or more argument(s) passed.
        };
    }
}
