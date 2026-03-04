using System.Runtime.InteropServices;

namespace EasyDockerFile.Core.Common;


public class SessionInfo() 
{
    public static Architecture HostArchitecture = RuntimeInformation.OSArchitecture;
}