using System.Runtime.InteropServices;

namespace EasyDockerFile.Core.Common;


public class SessionInfo 
{
    public readonly static Architecture HostArchitecture = RuntimeInformation.OSArchitecture;
}