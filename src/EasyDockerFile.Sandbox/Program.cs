
// // Test to ensure the docker images from Image.xml are properly serialized and the selection menu logic works as intended.
// using static EasyDockerFile.Core.Helpers.FamilyLoader;
// using static EasyDockerFile.Core.Helpers.InputHelper;
// using Spectre.Console;
// using EasyDockerFile.Core.Helpers;
// using EasyDockerFile.Core.Extensions;


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

// Console.WriteLine("{0}: {1}", nameof(familyChoice), familyChoice);
// Console.WriteLine("{0}: {1}", nameof(selectedImage), selectedImage!.FullName);
// Console.WriteLine($"Exiting Option Selected: {familyChoice.IsExitOption()}");


// Testing to ensure the Debian package manifest functionality works as intended.
// using EasyDockerFile.Core.API.PackageSearch;
// using System.Runtime.InteropServices;

// var debianPackageApi = new DebianPackageApi(Architecture.X64);
// await debianPackageApi.Load();

// foreach (var manifest in debianPackageApi.PackageManifests) {
//     Console.WriteLine(manifest);
// }



// // ZChunk binary loading test
// using EasyDockerFile.Core.Helpers;

// var location = ZChunkLoader.Load();
// Console.WriteLine(location);


// // Testing to ensure the Fedora package manifest functionality works as intended.
// using EasyDockerFile.Core.API.PackageSearch;
// using static System.Runtime.InteropServices.Architecture;

// var fedoraPackageApi = new FedoraPackageApi(X64, "43");
// await fedoraPackageApi.Load();
// Console.WriteLine("[SUCCESS]: Loaded {0} packages", fedoraPackageApi.PackageManifests.Count);

// foreach (var manifest in fedoraPackageApi.PackageManifests) {
//     Console.WriteLine(manifest);
// }