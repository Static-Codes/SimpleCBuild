using System.Text;
using static EasyDockerFile.Core.Common.Constants;

namespace EasyDockerFile.Core.API.PackageSearch.Manifests;

// Create panels for each of these sections.
public class DebianManifest 
{
    // --- Main Identification ---
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? Architecture { get; set; }
    public string? Source { get; set; }

    // --- Installation Metadata ---
    public string? FileName { get; set; }
    public string? DownloadSizeInBytes { get; set; }
    public string? InstallSizeKiB { get; set; }

    // --- Dependency Graph ---
    public string? PreDepends { get; set; }
    public string? Depends { get; set; }
    public string? Replaces { get; set; }
    public string? Provides { get; set; }
    public string? Suggests { get; set; }
    public string? Breaks { get; set; }

    // --- Project Information ---
    public string? Section { get; set; }
    public string? Priority { get; set; }
    public string? MultiArch { get; set; }
    public string? Maintainer { get; set; }
    public string? Homepage { get; set; }

    // --- Description + Tag ---
    public string? Description { get; set; }
    public string? Tag { get; set; }

    // --- Package Checksums ---
    public string? SHA256 { get; set; }
    public string? MD5Sum { get; set; }
    public string? DescriptionMD5 { get; set; }

    public override string ToString() 
    {
        var properties = GetType().GetProperties(_publicInstanceFlag);
        var stringBuilder = new StringBuilder();
        
        foreach (var prop in properties) 
        {
            var value = prop.GetValue(this) ?? "N/A";
            stringBuilder.AppendLine($"{prop.Name}: {value}");
        }
        
        stringBuilder.AppendLine();
        return stringBuilder.ToString();
    }
}