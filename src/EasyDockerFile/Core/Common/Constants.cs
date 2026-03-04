using System.Reflection;

namespace EasyDockerFile.Core.Common;

public class Constants 
{
    public readonly static string NLC = Environment.NewLine;

    public const BindingFlags _privateFlag = BindingFlags.NonPublic;
    public const BindingFlags _privateStaticFlag = BindingFlags.NonPublic | BindingFlags.Static;
    public const BindingFlags _publicFlag = BindingFlags.Public;
    public const BindingFlags _publicInstanceFlag = _publicFlag | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
}