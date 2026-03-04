using System.Runtime.InteropServices;
using EasyDockerFile.Core.API.PackageSearch;

var debianPackageApi = new DebianPackageApi(Architecture.X64);
await debianPackageApi.InitializeManifestList();

foreach (var manifest in debianPackageApi.PackageManifests) {
    Console.WriteLine(manifest);
}