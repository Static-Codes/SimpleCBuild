using EasyDockerFile.Core.Types;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using static EasyDockerFile.Core.Common.RequestManager.NetworkClient;
using static EasyDockerFile.Core.Helpers.DirectoryHelper;
using static Global.Logging;
using static System.Runtime.InteropServices.Architecture;


namespace EasyDockerFile.Core.API.PackageSearch.Mappers;

public class PackageMapping
{
    public (string Prefix, PackageType Type)[] typeMappings =
    [
        ("admin/", PackageType.Admin), 
        ("boot/", PackageType.Boot), 
        ("cli-mono/", PackageType.Mono),
        ("comm/", PackageType.Common), 
        ("database/", PackageType.Database), 
        ("debug/", PackageType.Debug),
        ("devel/", PackageType.GeneralDevelopment), 
        ("doc/", PackageType.Documentation), 
        ("editors/", PackageType.Editor),
        ("education/", PackageType.Education), 
        ("electronics/", PackageType.Electronics), 
        ("embedded/", PackageType.Electronics),
        ("fonts/", PackageType.Electronics), 
        ("games/", PackageType.Games), 
        ("gnome/", PackageType.Gnome),
        ("gnu-r/", PackageType.GnuR), 
        ("gnustep/", PackageType.GnuStep), 
        ("golang/", PackageType.Golang),
        ("graphics/", PackageType.Graphics), 
        ("haskell/", PackageType.Haskell), 
        ("hamradio/", PackageType.HamRadio),
        ("httpd/", PackageType.HttpD), 
        ("interpreters/", PackageType.Interpreters), 
        ("introspection/", PackageType.Interpreters),
        ("java/", PackageType.Java), 
        ("javascript/", PackageType.JavaScript), 
        ("kde/", PackageType.KDE),
        ("kernel/", PackageType.Kernel), 
        ("oldlibs/", PackageType.LegacyLibrary), 
        ("libdevel/", PackageType.LibraryDevelopment),
        ("libs/", PackageType.Library), 
        ("lisp/", PackageType.Lisp), 
        ("localization/", PackageType.Localization),
        ("mail/", PackageType.Mail), 
        ("math/", PackageType.Math), 
        ("metapackages/", PackageType.MetaPackages),
        ("misc/", PackageType.Misc), 
        ("net/", PackageType.Networking), 
        ("news/", PackageType.News),
        ("ocaml/", PackageType.Ocaml), 
        ("otherosfs/", PackageType.OtherOSFS), 
        ("perl/", PackageType.Perl), 
        ("php/", PackageType.PHP), 
        ("python/", PackageType.Python), 
        ("ruby/", PackageType.Ruby),
        ("rust/", PackageType.Rust), 
        ("science/", PackageType.Science), 
        ("shells/", PackageType.Shells),
        ("sound/", PackageType.Sound), 
        ("tex/", PackageType.Tex), 
        ("text/", PackageType.Text),
        ("utils/", PackageType.Utility), 
        ("vcs/", PackageType.VCS), 
        ("video/", PackageType.Video),
        ("web/", PackageType.Web), 
        ("x11/", PackageType.X11), 
        ("xfce/", PackageType.XFCE),
        ("zope/", PackageType.Zope)
    ];

    public IEnumerable<string> Paths { get; set; } = [];
    public List<string> PackageOutput { get; set; } = [];
    public List<Package> ParsedPackageList { get; } = [];
    public List<Package> ParsePackages()
    {
        for (int i = 0; i < PackageOutput.Count; i++)
        {
            string current = PackageOutput[i];

            // Removing the second package (if present in the manifest line), as it normally is unsigned.
            if (current.Contains(", ")) {
                current = current[current.IndexOf(", ")..];
            }

            // Skipping silverjuke sound packages during development as they will almost definitely not be used by the end user.
            #if DEBUG
                if (current.EndsWith("sound/silverjuke")) {
                    continue;
                }
            #endif

            if (current.Contains("  .")) {
                current = current.Replace("  .", " .");
            }

            bool matched = false;

            foreach (var mapping in typeMappings)
            {
                if (current.StartsWith(mapping.Prefix))
                {
                    ParsedPackageList.Add(new Package()
                    {
                        Type = mapping.Type,
                        Name = current[mapping.Prefix.Length..]
                    });
                    matched = true;
                    break;
                }
            }

            if (matched) {
                continue;
            }

            // Skipping the 3 known edge cases with sound/silverjuke
            // ->   usr/share/silverjuke/vis/bdrv + al Phat and Eo.S. _ shapes are cool_slow_colour_melting_edit BDRV et AL rmxmix32.milk
            // ->   usr/share/silverjuke/vis/LuxXx - GrindFace 225 mg dose .milk
            // ->   usr/share/silverjuke/vis/Rovastar - Kalideostars (Round Round Mix).milk 

            if (current.EndsWith("sound/silverjuke")) {
                continue;
            }

            ParsedPackageList.Add(new Package()
            {
                Type = PackageType.Unknown,
                Name = current
            });
        }

        return ParsedPackageList;
    }
}

public class Package
{
    public PackageType Type { get; set; } = PackageType.Unknown;
    public string Name { get; set; } = string.Empty;
}



public class ContentManifestInformation(FileStream contentStream)
{
    private static readonly string ExpectedContentPath = GetExpectedContentPath();
    public bool IsDownloaded { get; set; } = File.Exists(ExpectedContentPath);
    public string FilePath { get; set; } = contentStream.Name;
    public byte[] Contents { get; set; } = SafelyReadStreamBytes(contentStream);
    
    private static byte[] SafelyReadStreamBytes(FileStream contentStream) 
    {

        byte[] bytes = [];
        StreamReader? streamReader = null;
        try 
        {
            streamReader = new StreamReader(contentStream);
            bytes = JsonSerializer.Deserialize<byte[]>(contentStream) ?? [];
        }

        catch (Exception ex) {
            WriteWarningMessage("Unable to safely read bytes from the content manifest.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }

        finally 
        {
            streamReader?.Dispose();
        }

        return bytes;
    }

    private static string GetExpectedContentPath() 
    {
        var appDataSubDir = GetSCBAppDataDirectory();
        return Path.Combine(appDataSubDir, "Cache", "Manifest");
    }


}

public partial class PackageMapper 
{
    private static async Task<FileStream> GetDebianContentManifestStreamAsync(
        [AllowedValues([X64, Arm64])] 
        [DeniedValues(Arm, Armv6, LoongArch64, Ppc64le, RiscV64, S390x, Wasm, X86)]
        [DefaultValue(X64)] 
        Architecture guestArchitecture
    ) 
    {
        var contentsURL = guestArchitecture switch {
            X64 => "https://ftp.debian.org/debian/dists/trixie/main/Contents-amd64.gz",
            Arm64 => "https://ftp.debian.org/debian/dists/trixie/main/Contents-arm64.gz",
            _ => throw new ArgumentException("Invalid value provided to guestArchitecture in GetDebianManifestContent()")
        };

        Stream? compressedStream = null;

        WriteInformation("Downloading the compressed contents manifest", coloredText: "(~12MB)", textColor: "orange");
        try {
            compressedStream = await Instance.GetStreamAsync(contentsURL);

            if (compressedStream == null) {
                WriteWarningMessage("Unable to download the compressed Debian contents manifest.");
                WriteErrorMessage("Stream contents is null in GetDebianManifestContent()", exitCode: 1, exit: true);
            }

            WriteSuccessMessage("Downloaded complete.");
        }
        catch (Exception ex) {
            WriteWarningMessage("Unable to download the compressed Debian contents manifest.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }

        
        // Add check if 400MB of storage is free.

        WriteInformation("Decompressing the compressed contents manifest", coloredText: "(~162MB)", textColor: "orange");
        using var decompressor = new GZipStream(compressedStream, CompressionMode.Decompress);
        var tempFile = Path.GetTempFileName();
        FileStream? dataStream = null;
        try {
            dataStream = File.Create(tempFile);
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));
        }
        catch (Exception ex) {
            WriteWarningMessage("Decompression failed.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }


        try {
            await decompressor.CopyToAsync(dataStream!, 100000);
            WriteSuccessMessage("Decompression complete.");
        }

        catch (Exception ex) {
            WriteWarningMessage("Decompression failed.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }

        finally {
            File.Delete(tempFile);
        }

        dataStream.Position = 0;
        WriteSuccessMessage("Stream position reset.");


        
        return dataStream;
        
    }

    public static async Task<PackageMapping[]> GetMappings(Architecture guestArchitecture) 
    {   
        var manifestStream = await GetDebianContentManifestStreamAsync(guestArchitecture);

        // TODO: Add RAM check for manifestStream.Length * 2 && RAM_FREE => 8GB
        // TODO: Add ROM Check for manifestStream.Length * 2
        // TODO: Add CPU Core Check (Minimum 4 Core)

        using var reader = new StreamReader(manifestStream);
        var manifestContents = reader.ReadToEnd();

        return [.. manifestContents
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => {
                var parts = ManifestRegex().Split(line.Trim());

                var mapping = new PackageMapping 
                {
                    Paths = parts[0].Contains(' ') ? [.. 
                        parts[0]
                        .Split(' ') //
                        .Where(part => part.Contains('/')) // Ensuring the path contains a trailing slash.
                    ] : [parts[0]],

                    PackageOutput = parts.Length > 1 ? [.. 
                        parts[^1]   // Taking the member at the last index handles cases of unexpected naming schemes.
                        .Split(',') // Splitting the names which are comma separated.
                        .Where(part => !part.Contains("-unsigned")) // Removing unsigned packages for security.
                    ] : []
                };
                mapping.ParsePackages();
                return mapping;
            })];
            
    }

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex ManifestRegex();
}