#region Test to ensure the docker images from Image.xml are properly serialized and the selection menu logic works as intended.
// using static EasyDockerFile.Core.Helpers.InputHelper;
// using static EasyDockerFile.Core.Loaders.FamilyLoader;
// using Spectre.Console;
// using EasyDockerFile.Core.Helpers;
// using EasyDockerFile.Core.Extensions;
// using DockerFileSharp.Common;


// // Retrieving families
// var families = GetFamilies();


// // Choosing image family
// var familyNames = families.Select(fam => fam.Name);
// var familyChoice = AskForInput(
//     message: "Please select your desired image family.", 
//     options: MakeInputMenu(familyNames)
// );
// UserExitStatusCheck(familyChoice);

// var family = families.GetFamily(familyChoice);
// CheckForNullInput(family);


// // Choosing image version
// var imageNames = family.Images.Select(a => a.FullName);
// CheckForNullInput(imageNames);

// var imageChoice = AskForInput(
//     message: "Please select your desired image version.", 
//     options: MakeInputMenu(imageNames!)
// );
// UserExitStatusCheck(imageChoice);

// var selectedImage = family.Images.GetImage(imageChoice);
// CheckForNullInput(selectedImage);

// // Creating a DockerImage instance with the selected Image object
// var selectedDockerImage = new DockerImage(selectedImage);

// Console.WriteLine("{0}: {1}", nameof(familyChoice), familyChoice);
// Console.WriteLine("{0}: {1}", nameof(selectedImage), selectedImage!.FullName);
// Console.WriteLine($"Exiting Option Selected: {familyChoice.IsExitOption()}");
// Console.WriteLine("{0}: {1}", nameof(selectedDockerImage.ImageName), selectedDockerImage.ImageName);
#endregion

// #region Test to ensure the Debian package manifest functionality works as intended (This was removed but this test exists for future reference).
// using EasyDockerFile.Core.API.PackageSearch;
// using System.Runtime.InteropServices;

// var debianPackageApi = new DebianPackageApi(Architecture.X64);
// await debianPackageApi.Load();

// foreach (var manifest in debianPackageApi.PackageManifests[..10]) {
//     Console.WriteLine(manifest);
// }
// #endregion



#region "Testing CLI Commands + Repo Info"
using EasyDockerFile.Core.Common.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<MainMenuCommand>();

var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // Preventing the process from terminating immediately
    cancellationTokenSource.Cancel();
    AnsiConsole.WriteLine("[red]Cancellation requested...[/]");
    Environment.Exit(1);
};

app.Configure(config =>
{
    config.SetApplicationName("SimpleCBuild");
});

return await app.RunAsync(args, cancellationTokenSource.Token);
#endregion

#region "Ongoing tests for meson.build parsing.
// using EasyDockerFile.Core.API.RepoParser;

// MesonBuildParser.ParseBuildFile("/home/nerdy/Downloads/meson.build");

#endregion

#region Autotools to CMake conversion testing.
// using EasyDockerFile.Core.Conversion;
// using static EasyDockerFile.Core.Types.Inspections.CMake.CodeModelTypes;

// var projectDirectory = "/home/nerdy/repos/SCB/BuildSystemTests/Autotools/gnupg";
// var converter = new AutotoolsConverter(projectDirectory);
// var processedModelList = converter.TranslateToCMakeAndInspect();
// Console.WriteLine(processedModelList?.Count.ToString() ?? "null");


#endregion

#region "Docker Desktop Loading test"

// using static EasyDockerFile.Core.Loaders.DockerDesktopLoader;

// LoadDockerDesktop(); 

# endregion

# region "Testing how much data the application is using at startup"

// var bytesToMebibytesFactor = Math.Pow(1024, 2);
// Console.WriteLine(bytesToMebibytesFactor);
// Console.WriteLine(Environment.WorkingSet);
// Console.WriteLine(Environment.WorkingSet / bytesToMebibytesFactor);
// Environment.Exit(1);

# endregion


# region "Testing header to package name mapping"

// using EasyDockerFile.Core.API.PackageSearch.Mappers;
// using EasyDockerFile.Core.Helpers;
// using EasyDockerFile.Core.Types.System;

// var mappings = await PackageMapper.GetMappings(System.Runtime.InteropServices.Architecture.Arm64);


// #if DEBUG
//     DebugHelper.RunTests(mappings);
// #endif
#endregion


// #region "Testing SystemInfo"
// using EasyDockerFile.Core.Types.System;
// var systemInfo = new SystemInfo();

// RAMKit? value = systemInfo.MemoryInfo.HasValue ? systemInfo.MemoryInfo.Value.KitInfo : null;
// #endregion
