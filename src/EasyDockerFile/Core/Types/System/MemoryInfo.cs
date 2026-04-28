using EasyDockerFile.Core.Extensions;
using Hardware.Info;
using static EasyDockerFile.Core.Types.System.RAMCapacity;

namespace EasyDockerFile.Core.Types.System;


public struct MemoryInfo
{
    public RAMKit KitInfo { get; set; }
    public ulong TotalMemoryBytes { get; set; } = 0;
    public ulong AvailableMemoryBytes { get; set; } = 0;
    public ulong UsedMemoryBytes { get; set; } = 0;
    public bool HasSwapSpace { get; set; }
    public string MemoryConfiguration { get; set; } = "Unknown";
    
    /// <summary> 
    /// Processes the memory information from the associated IHardwareInfo object. <br/>
    /// Assigns values to TotalMemoryBytes, AvailableMemoryBytes, and UsedMemoryBytes.
    /// </summary>
    private RAMKit ProcessMemorySticks(IHardwareInfo hardwareInfo) 
    {
        // var identicalCapacity = true;
        // var identicalSpeed = true;
        // Memory? previousStick = null;

        var ramSticks = new RAMStick[hardwareInfo.MemoryList.Count];

        // This only iterates once.
        for (int i = 0; i < hardwareInfo.MemoryList.Count; i++) 
        {
            ramSticks[i] = new RAMStick() {
                Index = (uint)i,
                Capacity = MapCapacity(hardwareInfo.MemoryList[i]),
                TransferSpeed = hardwareInfo.MemoryList[i].Speed
            };

            // // Comparing the capacity of the previous stick to the current stick in the iteration.
            // if (previousStick != null && previousStick.Capacity != hardwareInfo.MemoryList[i].Capacity) {
            //     identicalCapacity = false;
            // }

            // if (previousStick != null && previousStick.Speed != hardwareInfo.MemoryList[i].Speed) {
            //     identicalSpeed = false;
            // }

            // previousStick = hardwareInfo.MemoryList[i];
            TotalMemoryBytes += hardwareInfo.MemoryList[i].Capacity;
        }


        AvailableMemoryBytes += hardwareInfo.MemoryStatus.AvailablePhysical + hardwareInfo.MemoryStatus.AvailableVirtual;
        UsedMemoryBytes = TotalMemoryBytes - AvailableMemoryBytes;

        return new RAMKit() {
            Sticks = ramSticks,
            // SameCapacity = identicalCapacity,
            // SameSpeed = identicalSpeed,
        };
    }

    

    private static RAMCapacity MapCapacity(Memory memory) 
    {
        if (memory.Capacity >= _4GB.ToLong() && memory.Capacity < _8GB.ToLong()) {
            return _4GB;
        }

        if (memory.Capacity >= _8GB.ToLong() && memory.Capacity < _16GB.ToLong()) {
            return _8GB;
        }

        if (memory.Capacity >= _16GB.ToLong() && memory.Capacity < _32GB.ToLong()) {
            return _16GB;
        }

        if (memory.Capacity >= _32GB.ToLong() && memory.Capacity < _64GB.ToLong()) {
            return _32GB;
        }

        if (memory.Capacity >= _64GB.ToLong() && memory.Capacity < _128GB.ToLong()) {
            return _64GB;
        }

        if (memory.Capacity >= _128GB.ToLong() && memory.Capacity < _256GB.ToLong()) {
            return _128GB;
        }

        if (memory.Capacity >= _256GB.ToLong() && memory.Capacity < _512GB.ToLong()) {
            return _256GB;
        }

        if (memory.Capacity >= _512GB.ToLong() && memory.Capacity < _1TB.ToLong()) {
            return _512GB;
        }

        if (memory.Capacity >= _1TB.ToLong()) {
            return _1TB;
        }

        return _UNKNOWN_OR_UNSUPPORTED;
    }

    private static uint SetLowestStickSpeed(RAMKit? kit) 
    {
        if (!kit.HasValue) {
            return 0;
        }

        uint lowestSpeed = 0; 
        var speeds = kit.Value.Sticks.Select(stick => stick.TransferSpeed);

        foreach (var speed in speeds) 
        {
            if (speed > lowestSpeed) {
                lowestSpeed = speed;
            }
        }

        return lowestSpeed;
    }
    
    private void SetMemoryConfiguration(RAMKit KitInfo) 
    {

        if (TotalMemoryBytes == 0) {
            return;
        }

        if (KitInfo.LowestStickSpeed == 0 || KitInfo.Sticks.Length == 0) {
            MemoryConfiguration = TotalMemoryBytes.ToGBString();
            return;
        }

        MemoryConfiguration = $"{TotalMemoryBytes.ToGBString()} @ {KitInfo.LowestStickSpeed} MT/s";
    }

    public MemoryInfo(IHardwareInfo? hardwareInfo) 
    {
        if (hardwareInfo == null) {
            throw new ArgumentNullException(
                nameof(hardwareInfo), 
                "Please pass an instance of IHardwareInfo to the constructor of MemoryInfo()"
            );
        }

        hardwareInfo.RefreshMemoryStatus();
        hardwareInfo.RefreshMemoryList();
        KitInfo = ProcessMemorySticks(hardwareInfo);
        SetLowestStickSpeed(KitInfo);
        SetMemoryConfiguration(KitInfo);
        // Console.WriteLine(MemoryConfiguration);
        // Console.WriteLine(KitInfo);
    }

    public MemoryInfo() 
    {
        throw new ArgumentNullException(
            "hardwareInfo", 
            "Please pass an instance of IHardwareInfo to the constructor of MemoryInfo()"
        );
    }
}