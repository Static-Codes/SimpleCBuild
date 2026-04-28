using EasyDockerFile.Core.API.PackageSearch.Base;
using EasyDockerFile.Core.API.PackageSearch.Manifests;
using EasyDockerFile.Core.API.PackageSearch.States;
using System.Buffers.Text;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using static EasyDockerFile.Core.Common.RequestManager.NetworkClient;
using static System.Runtime.InteropServices.Architecture;

namespace EasyDockerFile.Core.API.PackageSearch;

// <summary>
// Implementation of PackageSearchApi for Debian platforms.
// <param name="architecture"> The desired architecture for the returned packages </param>
// </summary>
public class DebianPackageApi(Architecture architecture) : IPackageSearchApi 
{
    public override string? PlatformIdentifer => GetPlatformIdentifer(architecture);
    public override string? PackageFileUri => GetUri(architecture);
    public override string[] FallbackPackages => ["git", "curl", "wget", "nano", "vim", "build-essentials", "libc-dev"];
    public List<DebianManifest> PackageManifests = [];
    

    private async Task<FileStream?> DownloadManifestFile() 
    {
        Console.WriteLine("[INFO]: Downloading compressed Debian package manifest (~13MB)");
        var compressedStream = await Instance.GetStreamAsync(PackageFileUri!);
        Console.WriteLine("[SUCCESS]: Downloaded successful");
        
        // Add check if 150MB of storage is free.

        Console.WriteLine("[INFO]: Decompressing the compressed package manifest (~50MB)");
        using var decompressor = new GZipStream(compressedStream, CompressionMode.Decompress);
        var tempFile = Path.GetTempFileName();
        FileStream? dataStream = null;
        try {
            dataStream = File.Create(tempFile);
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));
        }
        catch (Exception ex) {
            Console.WriteLine("[WARNING]: Decompression failed");
            Console.WriteLine($"[ERROR]: {ex.Message}");
            Environment.Exit(1);
        }


        await decompressor.CopyToAsync(dataStream!, 100000);
        Console.WriteLine("[SUCCESS]: Decompression successful");

        Console.WriteLine("[SUCCESS]: Reset stream position");
        dataStream.Position = 0;
        return dataStream;
    }

    private static void FlushField(ReadOnlySpan<byte> block, ref ManifestParserState state, DebianManifest manifest)
    {
        if (state.HasPendingField)
        {
            var valSpan = block.Slice(state.ValueStart, state.ValueLength);
            MapField(manifest, state.Key, valSpan);
            state.Clear();
        }
    }

    private static Dictionary<int, ReadOnlyMemory<byte>> GetPackageDictionary(ReadOnlyMemory<byte> buffer)
    {
        var packageData = new Dictionary<int, ReadOnlyMemory<byte>>();

        if (buffer.IsEmpty) {
            return packageData;
        }

        int packagesFound = 0;
        int currentStartIndex = 0;
        ReadOnlySpan<byte> span = buffer.Span;

        while (currentStartIndex < span.Length)
        {
            // Slice the remaining data to search within
            ReadOnlySpan<byte> remaining = span[currentStartIndex..];
            
            // Find the next blank line (\n\n)
            int relativeIndex = remaining.IndexOf("\n\n"u8);

            if (relativeIndex == -1) 
            {
                // Capture the final package block
                ReadOnlyMemory<byte> lastSlice = buffer[currentStartIndex..];
                if (IsWhiteSpace(lastSlice.Span)) {
                    packageData.Add(packagesFound++, lastSlice);
                }
                break;
            }

            // Determine if we are dealing with \r\n\n or just \n\n
            // This ensures we don't leave a trailing \r in the package memory
            int sliceLength = relativeIndex;
            if (relativeIndex > 0 && remaining[relativeIndex - 1] == (byte)'\r')
            {
                sliceLength--; 
            }

            // Add the package block to the dictionary
            packageData.Add(packagesFound++, buffer.Slice(currentStartIndex, sliceLength));

            // Advance: move past the found index + the 2 bytes of '\n\n'
            currentStartIndex += relativeIndex + 2;
        }

        return packageData;
    }

    private static string? GetPlatformIdentifer(Architecture arch) => arch switch {
        X64 => "amd64", 
        Arm64 => "arm64", 
        Arm => "armhf",
        _ => null,
    };

    private static string GetUri(Architecture arch)
    {
        // Returns the Platform identifier associated with the specified architecture
        // Source: https://www.debian.org/releases/stable/amd64/ch02s01.en.html
        var PlatformID = GetPlatformIdentifer(arch);

        ArgumentNullException.ThrowIfNull(PlatformID);

        var uri = $"http://deb.debian.org/debian/dists/stable/main/binary-{PlatformID}/Packages.gz";
        return uri;
    }

    /// <summary>
    /// <para> Initializes <DebianPackageApi>.PackageManifests
    /// This must be called before accessing <DebianPackageApi>.PackageManifests
    /// </summary>
    public async Task InitializeManifestList() 
    {
        FileStream? manifestStream = await DownloadManifestFile();
        var readOnlyManifestBytes = await GetReadOnlyFileMemory(manifestStream);

        var data = GetPackageDictionary(readOnlyManifestBytes);
        PackageManifests = ParsePackageDictionary(data);
    }
    

    private static async Task<ReadOnlyMemory<byte>> GetReadOnlyFileMemory(FileStream? dataStream) 
    {
        var buffer = new byte[dataStream!.Length];

        try{
            await dataStream.ReadExactlyAsync(buffer.AsMemory());
            dataStream.Dispose();
        }
        catch (Exception ex) {
            Console.WriteLine("[WARNING]: Failed to read manifest buffer contents.");
            Console.WriteLine($"[ERROR TYPE]: {ex.GetType()}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
            Console.WriteLine($"[STACKTRACE]: {ex.StackTrace}");
            Environment.Exit(1);
        }

        var readOnlyMemory = new ReadOnlyMemory<byte>(buffer);
        return readOnlyMemory;
    }

    private static bool IsWhiteSpace(ReadOnlySpan<byte> span)
    {
        foreach (var b in span)
        {
            // Includes:
            // 0x09: Horizontal Tab (\t)
            // 0x0A: Line Feed (\n)
            // 0x0B: Vertical Tab (VT)
            // 0x0C: Form Feed (FF)
            // 0x0D: Carriage Return (\r)
            // 0x20: Space
            // Any byte > 32 is a printable character (e.g., '!', 'A', '1')
            if (b > 32) {
                return false;
            } 
        }
        return true;
    }

    private static void MapField(DebianManifest manifest, ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
    {
        // --- Main Identification ---
        if (key.SequenceEqual("Package"u8)) {
            manifest.Name = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Version"u8)) {
            manifest.Version = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Architecture"u8)) {
            manifest.Architecture = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Source"u8)) {
            manifest.Source = Encoding.UTF8.GetString(value);
        }

        // --- Installation Metadata ---
        else if (key.SequenceEqual("Filename"u8)) {
            manifest.FileName = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Size"u8)) {
            manifest.DownloadSizeInBytes = Encoding.UTF8.GetString(value);
        }

        else if (key.SequenceEqual("Installed-Size"u8)) {
            var computed = Utf8Parser.TryParse(value, out long size, out _);
            manifest.InstallSizeKiB = computed ? size.ToString() : "Unable to compute";
        }


        // --- Dependency Graph ---
        else if (key.SequenceEqual("Pre-Depends"u8)) {
            manifest.PreDepends = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Depends"u8)) {
            manifest.Depends = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Replaces"u8)) {
            manifest.Replaces = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Provides"u8)) {
            manifest.Provides = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Suggests"u8)) {
            manifest.Suggests = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Breaks"u8)) {
            manifest.Breaks = Encoding.UTF8.GetString(value);
        }

        // --- Project Information ---
        else if (key.SequenceEqual("Section"u8)) {
            manifest.Section = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Priority"u8)) {
            manifest.Priority = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Multi-Arch"u8)) {
            manifest.MultiArch = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Maintainer"u8)) {
            manifest.Maintainer = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Homepage"u8)) {
            manifest.Homepage = Encoding.UTF8.GetString(value);
        }

        // --- Description + Tag ---
        else if (key.SequenceEqual("Description"u8)) {
            manifest.Description = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Tag"u8)) {
            manifest.Tag = Encoding.UTF8.GetString(value);
        }

        // --- Package Checksums ---
        else if (key.SequenceEqual("SHA256"u8)) {
            manifest.SHA256 = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("MD5sum"u8)) {
            manifest.MD5Sum = Encoding.UTF8.GetString(value);
        }
        else if (key.SequenceEqual("Description-md5"u8)) {
            manifest.DescriptionMD5 = Encoding.UTF8.GetString(value);
        }
    }

    private static List<DebianManifest> ParsePackageDictionary(Dictionary<int, ReadOnlyMemory<byte>> packageDatabase)
    {
        var packageData = new List<DebianManifest>(packageDatabase.Count);

        foreach (var entry in packageDatabase)
        {
            var manifest = ParseSingleManifest(entry.Value.Span);
            packageData.Add(manifest);
        }

        return packageData;
    }

    private static DebianManifest ParseSingleManifest(ReadOnlySpan<byte> block)
    {
        var manifest = new DebianManifest();
        ManifestParserState state = default;

        int lineStart = 0;
        int lineEnd;

        while (lineStart < block.Length)
        {
            // Parses the entire block for the index of the expected newline char.
            // If the expected newline char is not found, this will equal -1
            lineEnd = block[lineStart..].IndexOf((byte)'\n');

            // NLC found -> the entire block length is used, excluding the trailing new line char.
            // NLC missing -> the entire block is used.
            int currentLineLength = (lineEnd == -1) ? block.Length - lineStart : lineEnd;

            ReadOnlySpan<byte> line = block.Slice(lineStart, currentLineLength);

            // Handles the line logic
            ParseManifestLine(block, line, lineStart, ref state, manifest);

            if (lineEnd == -1) {
                break;
            }

            lineStart += lineEnd + 1;
        }

        FlushField(block, ref state, manifest);
        return manifest;
    }

    private static void ParseManifestLine(ReadOnlySpan<byte> block, ReadOnlySpan<byte> line, int lineOffset, ref ManifestParserState state, DebianManifest manifest)
    {
        var isReturnCarriage = line.Length == 1 && line[0] == (byte)'\r';
        var lineStartsWithSpace = line[0] == (byte)' ';
        var lineStartsWithTab = line[0] == (byte)'\t';
        var hasLineFolding = lineStartsWithSpace && lineStartsWithTab;
        
        if (line.IsEmpty || isReturnCarriage) {
            return;
        }

        var valueUpdateRequired = hasLineFolding && state.HasPendingField;

        if (valueUpdateRequired) {
            state.ValueLength = lineOffset + line.Length - state.ValueStart;
        }

        // Flusing the previous block if prior to a new entry
        FlushField(block, ref state, manifest);

        // Parsing the current lines key (seperated from the value by ': ')
        int colonIndex = line.IndexOf((byte)':');
        if (colonIndex != -1)
        {
            state.Key = line[..colonIndex];
            int valueOffset = colonIndex + 1;

            // Handles double commas (which are used in the tag section)
            // Also skips the leading space in the key's value.
            while (valueOffset < line.Length && (line[valueOffset] == (byte)':' || line[valueOffset] == (byte)' '))
            {
                valueOffset++;
            }

            state.ValueStart = lineOffset + valueOffset;
            state.ValueLength = line.Length - valueOffset;
        }
    }
    
}