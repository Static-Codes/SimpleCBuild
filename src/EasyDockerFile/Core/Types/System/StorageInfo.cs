using Hardware.Info;

namespace EasyDockerFile.Core.Types.System;

public struct StorageInfo 
{
    public StorageInfo(IHardwareInfo? hardwareInfo) 
    {
        if (hardwareInfo == null) {
            throw new ArgumentNullException(
                nameof(hardwareInfo), 
                "Please pass an instance of IHardwareInfo to the constructor of StorageInfo()"
            );
        }

        hardwareInfo.RefreshDriveList();
    }

    public StorageInfo() 
    {
        throw new ArgumentNullException(
            "hardwareInfo", 
            "Please pass an instance of IHardwareInfo to the constructor of StorageInfo()"
        );
    }
}