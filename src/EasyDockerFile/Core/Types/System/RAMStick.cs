namespace EasyDockerFile.Core.Types.System;
public struct RAMStick() 
{
    public uint Index { get; set; }
    public RAMCapacity Capacity { get; set; }
    public uint TransferSpeed { get; set; } = 0;
}