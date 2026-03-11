using EasyDockerFile.Core.API.PackageSearch.Base;
using EasyDockerFile.Core.API.PackageSearch.Manifests;
using EasyDockerFile.Core.Loaders;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using static EasyDockerFile.Core.Common.Constants;
using static EasyDockerFile.Core.Common.Platform;
using static EasyDockerFile.Core.Common.RequestManager.NetworkClient;
using static System.Runtime.InteropServices.Architecture;

namespace EasyDockerFile.Core.API.PackageSearch;

// <summary>
// Implementation of PackageSearchApi for Fedora-based distros.
// <param name="architecture"> The desired architecture for the returned packages </param>
// </summary>
public class FedoraPackageApi : PackageSearchApi 
{
    private readonly Task<string?> _getUriTask;
    public new string? PlatformIdentifer;
    
    public FedoraPackageApi(Architecture architecture, string fedoraVersion)
    {
        Console.WriteLine("[INFO]: Initializing _getUriTask");
        PlatformIdentifer = GetPlatformIdentifer(architecture);
        _getUriTask = InitializeGetUriTask(architecture, fedoraVersion);
    }
    
    // If for some reason the Task above did not complete as intended, the quick and dirty fix is .GetAwaiter().GetResult()
    public override string? PackageFileUri => _getUriTask.Status == TaskStatus.RanToCompletion 
        ? _getUriTask.Result 
        : _getUriTask.GetAwaiter().GetResult();



    public override string[] FallbackPackages => ["git", "curl", "wget", "nano", "vim"];
    public List<FedoraPackage> PackageManifests = [];
    

    private async Task<HttpResponseMessage> GetRepoMDResponse() 
    {
        if (string.IsNullOrEmpty(PackageFileUri)) {
            throw new InvalidOperationException("[EXCEPTION]: PackageFileUri has not been initialized.");
        }

        UpdateUserAgent("curl/8.5.0");
        var rawResponse = await Instance.GetAsync(PackageFileUri);
        ResetUserAgentToDefault();

        ArgumentNullException.ThrowIfNull(rawResponse);

        rawResponse.EnsureSuccessStatusCode();
        return rawResponse;
    }

    /// <summary>
    /// Returns the Platform identifier associated with the specified architecture <br/>
    /// Source: https://fedoraproject.org/wiki/Architectures
    /// </summary>
    private static string? GetPlatformIdentifer(Architecture arch) => arch switch {
        X64 => "x86_64", 
        Arm64 => "aarch64", 
        S390x => "s390x",
        Ppc64le => "ppc64le",
        _ => null,
    };

    /// <summary>
    /// Returns the path to the dnf metadata xml file.
    /// </summary>
    private static string GetMetadataXMLFile(Architecture arch, string fedoraVersion) {
        
        var PlatformID = GetPlatformIdentifer(arch);

        ArgumentNullException.ThrowIfNull(PlatformID);

        return $"https://dl.fedoraproject.org/pub/fedora/linux/releases/{fedoraVersion}/Everything/{PlatformID}/os/repodata/";
    }
    

    // Ensures all types that touch RepoMD are preserved, even if they arent explicitly used.
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(RepoMD))]
    // [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "Roots.xml handles the preservation of these types.")]
    // [UnconditionalSuppressMessage("AotAnalysis", "IL3050", Justification = "Due to the analysis above, currently, this method is AOT safe.")]
    private async Task<string?> InitializeGetUriTask(Architecture architecture, string fedoraVersion) 
    {   
        var repoDataRemoteDir = GetMetadataXMLFile(architecture, fedoraVersion);

        try 
        {
            var repoMDURL = Path.Combine(repoDataRemoteDir, "repomd.xml");

            UpdateUserAgent("curl/8.5.0");
            var repoXMLResponseMessage = await Instance.GetAsync(repoMDURL);
            ResetUserAgentToDefault();

            ArgumentNullException.ThrowIfNull(repoXMLResponseMessage);
            repoXMLResponseMessage.EnsureSuccessStatusCode();

            var xmlBytes = await repoXMLResponseMessage.Content.ReadAsByteArrayAsync();
            using var memoryStream = new MemoryStream(xmlBytes);
            // var serializer = new XmlSerializer(typeof(RepoMD));
            
            ArgumentNullException.ThrowIfNull(memoryStream, nameof(memoryStream));
            
            var doc = XDocument.Load(memoryStream);

            var repoXMLObj = FedoraXmlMapper.MapRepoMD(doc);

            // var repoXMLFileObj = serializer.Deserialize(memoryStream);
            // ArgumentNullException.ThrowIfNull(repoXMLFileObj, nameof(repoXMLFileObj));

            // var repoXMLObj = (RepoMD)repoXMLFileObj;

            var dataBlocks = repoXMLObj.Data;
            ArgumentNullException.ThrowIfNull(dataBlocks, nameof(dataBlocks));

            var desiredBlock = dataBlocks
                                .Where(block => block.Type == "primary_zck")
                                .FirstOrDefault();

            
            ArgumentNullException.ThrowIfNull(desiredBlock, nameof(desiredBlock));

            
            var baseUri = string.Format(
                "https://dl.fedoraproject.org/pub/fedora/linux/releases/{0}/Everything/{1}/os/",
                fedoraVersion,
                PlatformIdentifer
            );

            ArgumentNullException.ThrowIfNull(desiredBlock.Location);
            ArgumentNullException.ThrowIfNull(desiredBlock.Location.Href);


            var finalPackageUri = new Uri(new Uri(baseUri), desiredBlock.Location.Href);
            Console.WriteLine($"[SUCCESS]: Initialized _getUriTask with Uri: {finalPackageUri}");
            return finalPackageUri.ToString();
        }

        catch (Exception ex) {
            Console.WriteLine("[WARNING]: Unable to initialize _getUriTask");
            Console.WriteLine($"[ERROR TYPE]: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
        }

        return null;

    }

    /// <summary>
    /// Initializes FedoraPackageApi.PackageManifests <br/>
    /// This must be called before accessing FedoraPackageApi.PackageManifests
    /// </summary>
    public async Task Load() 
    {
        Console.WriteLine($"[INFO]: Downloading compressed Fedora package manifest from: {PackageFileUri}");

        var repoMDResponse = await GetRepoMDResponse();

        var tempXmlStream = await DecompressManifest(repoMDResponse);

        await ProcessManifestInChunksAsync(tempXmlStream);
    }

    // HasExecutablePermissions and SetExecutablePermissions are unix only
    // This CA1416 can be safely suppress as it is accounted for with the conditional below.
    [UnconditionalSuppressMessage("Interoperability", "CA1416")]
    private static bool SetExecutableFlag(string binaryPath) {
        if (IsWindows) {
            // Add actions later
            return false;
        } 
        
        if (!UnixFilePermissions.HasExecutablePermissions(binaryPath)) {
            return UnixFilePermissions.SetExecutablePermissions(binaryPath);
        }
        return false;
    }

    private static async Task<FileStream> DecompressManifest(HttpResponseMessage response)
    {
        var randomFileName = Guid.NewGuid().ToString();
        var baseFilePath = Path.Combine(TEMP_DIR, randomFileName);


        var tempZck = baseFilePath + ".xml.zck";
        var tempXml = baseFilePath + ".xml";



        var unzckBinary = ZChunkLoader.Load();

        #if DEBUG
            Console.WriteLine("[DEBUG]: Base FilePath: {0}", baseFilePath);
            Console.WriteLine("[DEBUG]: Temp XML Path: {0}", tempXml);
            Console.WriteLine("[DEBUG]: Temp ZCK Path: {0}", tempZck);
            Console.WriteLine("[DEBUG]: Unzck Binary Path: {0}", unzckBinary);
        #endif

        ArgumentNullException.ThrowIfNull(unzckBinary);
        
        try
        {
            var tempZckStream = new FileStream(tempZck, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
            await response.Content.CopyToAsync(tempZckStream, token);
            
            #if DEBUG
                Console.WriteLine($"[DEBUG]: Filesize: {(double)tempZckStream!.Length / Math.Pow(1024, 2)} MiB");
            #endif

            SetExecutableFlag(unzckBinary);
            
            // Due to the use of a direct binary invoke, it is not possible to redirect stdout to null
            // The message below is appended to the stdout that is written to form a correct path 
            Console.Write("[INFO]: Extracting to /tmp/");
            var startInfo = new ProcessStartInfo
            {
                FileName = unzckBinary,
                Arguments = $"\"{tempZck}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                WorkingDirectory = TEMP_DIR
            };

            // Console.WriteLine($"{unzckBinary} \"{tempZck}\"");
            using var process = Process.Start(startInfo);
            await process!.WaitForExitAsync();

            if (process.ExitCode != 0) {
                throw new Exception($"unzck failed: {await process.StandardError.ReadToEndAsync()}");
            }

            if (File.Exists(tempXml)) { 
                return new FileStream(tempXml, FileMode.Open, FileAccess.Read);
            }

        }

        catch (Exception ex) {
            Console.WriteLine("[WARNING]: Unable to decompress fedora manifest");
            Console.WriteLine($"[ERROR TYPE]: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
        }

        finally
        {
            // Deleting the zchunk file now that it is extracted.
            if (File.Exists(tempZck)) { 
                File.Delete(tempZck); 
            }

            // Deleting the unzck binary now that the above file is deleted.
            if (File.Exists(unzckBinary)) {
                File.Delete(unzckBinary);
            }
            
        }
        Environment.Exit(1);
        return null;

    }
    
    private async Task ProcessManifestInChunksAsync(FileStream? tempXMLStream, int chunkSize = 100)
    {
        ArgumentNullException.ThrowIfNull(tempXMLStream);

        if (tempXMLStream.CanSeek) {
            tempXMLStream.Position = 0;
        }

        var currentChunk = new List<FedoraPackage>(chunkSize);

        using var reader = XmlReader.Create(tempXMLStream, new XmlReaderSettings { Async = true });
        try 
        {
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "package")
                {
                    // Deserializing the entire package object
                    var el = (XElement)XNode.ReadFrom(reader);

                    var pkg = FedoraXmlMapper.MapPackage(el);

                    if (pkg != null) {
                        currentChunk.Add(pkg);
                    }
                    
                    // Once the currentChunk has finished, the next x packages are loaded.
                    if (currentChunk.Count >= chunkSize)
                    {
                        ProcessManifestChunk(currentChunk);
                        currentChunk.Clear(); // Frees memory for the next x packages
                    }
                }
            }

            // Includes the last partial chunk, which was left out previously.
            if (currentChunk.Count > 0)
            {
                ProcessManifestChunk(currentChunk);
            }

            if (File.Exists(tempXMLStream.Name)) { 
                File.Delete(tempXMLStream.Name); 
            }

            Console.WriteLine("[SUCCESS]: Download successful");
        }

        catch (Exception ex) {
            Console.WriteLine("[WARNING]: Unable to process Fedora package manifest");
            Console.WriteLine($"[ERROR TYPE]: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
        }

        finally {
            // Disposes of the stream, which frees assocated memory, and also deletes temp files created by this stream/process
            await tempXMLStream.DisposeAsync();
        }
    }

    private void ProcessManifestChunk(List<FedoraPackage> currentChunk) 
    {
        PackageManifests.AddRange(currentChunk);
    }

}