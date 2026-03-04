using static EasyDockerFile.Core.Helpers.ImageLoader;
using static EasyDockerFile.Core.Helpers.InputHelper;
using Spectre.Console;


var families = GetFamilies();
var familyNames = families.Select(fam => fam.Name);

var familyChoice = AskForInput(
    message: "Please select your desired image family.", 
    options: MakeInputMenu(familyNames)
);

Console.WriteLine($"User's choice: {familyChoice}");
Console.WriteLine($"User wants to exit: {familyChoice.IsExitOption()}");



// var debianPackageApi = new DebianPackageApi(Architecture.X64);
// await debianPackageApi.InitializeManifestList();

// foreach (var manifest in debianPackageApi.PackageManifests) {
//     Console.WriteLine(manifest);
// }
