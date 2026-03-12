# region Test to ensure the docker images from Image.xml are properly serialized and the selection menu logic works as intended.
// using static EasyDockerFile.Core.Helpers.InputHelper;
// using static EasyDockerFile.Core.Loaders.FamilyLoader;
// using Spectre.Console;
// using EasyDockerFile.Core.Helpers;
// using EasyDockerFile.Core.Extensions;
// using EasyDockerFile.Core.Types.DockerTypes;


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
# endregion

#region Test to ensure the Debian package manifest functionality works as intended.
// using EasyDockerFile.Core.API.PackageSearch;
// using System.Runtime.InteropServices;

// var debianPackageApi = new DebianPackageApi(Architecture.X64);
// await debianPackageApi.Load();

// foreach (var manifest in debianPackageApi.PackageManifests[..10]) {
//     Console.WriteLine(manifest);
// }
#endregion

#region ZChunk binary loading test
// using EasyDockerFile.Core.Loaders;
// var location = ZChunkLoader.Load();
// Console.WriteLine(location);
#endregion

#region Testing to ensure the Fedora package manifest functionality works as intended.
// using EasyDockerFile.Core.API.PackageSearch;
// using static System.Runtime.InteropServices.Architecture;

// var fedoraPackageApi = new FedoraPackageApi(X64, "43");
// await fedoraPackageApi.Load();
// Console.WriteLine("[SUCCESS]: Loaded {0} packages", fedoraPackageApi.PackageManifests.Count);

// foreach (var manifest in fedoraPackageApi.PackageManifests[..10]) {
//     Console.WriteLine(manifest);
// }
#endregion

#region Testing RepoInfo

// using EasyDockerFile.Core.API.ToolchainSearch.Git;
// using EasyDockerFile.Core.Types.GitTypes;

// // public repo test
// // var repoInfoObj = new RepoInfo("https://github.com/Static-Codes/BrowserAutomationMaster");

// // private repo test
// var repoInfoObj = new RepoInfo("https://github.com/Static-Codes/bamm-install-hosted", args);

// // invalid repo test
// // var repoInfoObj = new RepoInfo("https://github.com/Static-Codes/BrowserAutomationMasterxcv");


// var client = new RepoClient(repoInfoObj);
// await client.UpdateBranchesAsync();
// client.UpdateStatus();
// Console.WriteLine(client);

#endregion


#region Testing CLI Commands

// using EasyDockerFile.Core.Common.Commands;
// using Spectre.Console;
// using Spectre.Console.Cli;

// var app = new CommandApp<MainMenuCommand>();

// var cancellationTokenSource = new CancellationTokenSource();
  
// Console.CancelKeyPress += (_, e) =>
// {
//     e.Cancel = true; // Preventing the process from terminating immediately
//     cancellationTokenSource.Cancel();
//     AnsiConsole.WriteLine("[red]Cancellation requested...[/]");
//     Environment.Exit(1);
// };

// app.Configure(config =>
// {
//     config.SetApplicationName("EasyDockerFile");
// });

// return await app.RunAsync(args, cancellationTokenSource.Token);
#endregion

#region "Ongoing tests for meson.build parsing.
using EasyDockerFile.Core.API.RepoParser.BuildSystem.Meson;

MesonProjectFunctions.ParseBuildFile("/home/nerdy/Downloads/meson.build");

#endregion


