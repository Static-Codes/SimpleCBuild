using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Helpers;
using Global.Build;
using Spectre.Console.Cli;
using static Global.Constants;

namespace EasyDockerFile.Core.Common.Commands;

// Resolving all members that touch MainMenuSettings, even if they are explicitly called.
// Resolves warning IL3050: 
// Using member 'Spectre.Console.Cli.CommandApp<TDefaultCommand>.CommandApp(ITypeRegistrar)' 
// which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. 
// Spectre.Console.Cli relies on reflection.
// Use during trimming and AOT compilation is not supported and may result in unexpected behaviors.
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class MainMenuSettings : CommandSettings
{
    [CommandOption("-p|--private")]
    [Description("Indicates the repository is private.")]
    [DefaultValue(false)]
    public bool PrivateFlagSet { get; init; }

    [CommandOption("-r|--repo|--repository <VALUE>")]
    [Description("Specifies the repository you want to build from.")]
    [DefaultValue(null)]
    public required string? RepoLink { get; init; }

    [CommandOption("-a|--arch|--architecture <VALUE>")]
    [Description("Specifies the architecture you want to build for.")]
    [DefaultValue(null)]
    public string? ArchitectureString { get; set; }


    [CommandOption("-o|--output <VALUE>")]
    [Description(
        "Specifies the output type for the compiled binary.\n" +
        "If this parameter is not passed, you will be prompted to make a selection.\n" +
        "This serves as an optional way to utilize SimpleCBuild without going through selection menus."
    )]
    [DefaultValue(null)]
    public string? OutputType { get; set; }


    [CommandOption("-t|--token <VALUE>")]
    [Description("Specifies the OAuth token.")]
    [DefaultValue(null)]
    public string? OAuthToken { get; init; }

    
    /// <summary>
    ///      <br/> Due to the requirement for [DefaultValue(type)] to be a compile time constant:
    ///      <br/> The following mutator method is required and will be invoked by the global `settings` variable in MainMenuCommand.
    ///      <br/> ArchitectureString is guaranteed to be a non-null value after the execution of this function.
    /// </summary>
    [MemberNotNull(nameof(ArchitectureString))]
    public void SetArchitectureString() 
    {
        ArchitectureString ??= InputHelper.AskForInput(
            message:"Please select your desired architecture to build for.", 
            options: ["x64", "arm64", "Exit"], 
            pageSize: 3
        );
    }

    /// <summary>
    ///      <br/> Due to the requirement for [DefaultValue(type)] to be a compile time constant:
    ///      <br/> The following mutator method is required and will be invoked by the global `settings` variable in MainMenuCommand.
    ///      <br/> OutputType is guaranteed to be a non-null value after the execution of this function.
    /// </summary>
    [MemberNotNull(nameof(OutputType))]
    public void SetOutputType() 
    {
        var foundTypes = 
            typeof(OutputTypes)
            .GetFields(_publicFlag)
            .Select(field => field.Name.Sanitize());
        
        var types = foundTypes.Any() ? foundTypes : 
        [
            "Executable",
            "Standalone Executable",
            "Shared Library", 
            "Static Library"
        ];
        
        
        OutputType ??= InputHelper.AskForInput(
            message: "Please select your desired output type.",
            options: types,
            pageSize: 3
        );
    }
}