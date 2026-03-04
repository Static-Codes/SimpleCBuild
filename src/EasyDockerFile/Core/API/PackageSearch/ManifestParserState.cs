namespace EasyDockerFile.Core.API.PackageSearch;
public ref struct ManifestParserState
{
    public ReadOnlySpan<byte> Key;
    public int ValueStart;
    public int ValueLength;
    public readonly bool HasPendingField => !Key.IsEmpty;

    public void Clear()
    {
        Key = default; // instead of reassigning the value manually, default is shorter.
        ValueStart = -1;
        ValueLength = 0;
    }
}