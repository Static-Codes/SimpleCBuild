namespace Global;

using System.Reflection;

internal class Constants 
{
    public static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    public const BindingFlags _privateFlag = BindingFlags.NonPublic;
    public const BindingFlags _privateStaticFlag = BindingFlags.NonPublic | BindingFlags.Static;
    public const BindingFlags _publicFlag = BindingFlags.Public;
    public const BindingFlags _publicInstanceFlag = _publicFlag | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
    public readonly static string TEMP_DIR = Path.GetTempPath();
    public readonly static string NLC = Environment.NewLine;
    public readonly static string[] MesonUnixSystemNames = ["darwin", "dragonfly", "freebsd", "linux", "netbsd", "openbsd"];
    public const string DebugTag = "[[DEBUG]]";
    public const string ErrorTag = "[[ERROR]]:";
    public const string InfoTag = "[[INFO]]:";
    public const string WarningTag = "[[WARNING]]:";
    public const string SuccessTag = "[[SUCCESS]]:";
    public const string OrangeHex = "#FFAF00";
}