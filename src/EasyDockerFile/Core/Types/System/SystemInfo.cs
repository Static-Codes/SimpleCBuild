using Hardware.Info;

namespace EasyDockerFile.Core.Types.System;

public struct SystemInfo() 
{
    public static IHardwareInfo? _HardwareInfo { get; set; } = new HardwareInfo();
    public MemoryInfo? MemoryInfo { get; set; } = new(_HardwareInfo);
    public ProcessorInfo? ProcessorInfo { get; set; } = new(_HardwareInfo);
    public StorageInfo? StorageInfo { get; set; } = new(_HardwareInfo);
}