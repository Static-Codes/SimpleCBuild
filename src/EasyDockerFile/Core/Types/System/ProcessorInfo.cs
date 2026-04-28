namespace EasyDockerFile.Core.Types.System;

using Hardware.Info;
using static Global.Logging;
public struct ProcessorInfo 
{

    private static CPU? CPUObject { get; set; }
    public static string Name { get; set; } = "Unknown";
    public static int? PhysicalCores { get; set; } = -1;
    public static int? LogicalProcessors { get; set; } = -1;

    public ProcessorInfo(IHardwareInfo? hardwareInfo) 
    {

        if (hardwareInfo == null) {
            throw new ArgumentNullException(
                nameof(hardwareInfo), 
                "Please pass an instance of IHardwareInfo to the constructor of ProcessorInfo()"
            );
        }

        if (!Refresh(hardwareInfo)) {
            WriteWarningMessage("Unable to refresh CPU information.");
            WriteErrorMessage("");
        }
    }

    public static bool Refresh(IHardwareInfo hardwareInfo) 
    {
        hardwareInfo.RefreshCPUList();
        CPUObject = hardwareInfo.CpuList.FirstOrDefault();
        return CPUObject != null;
    }
}