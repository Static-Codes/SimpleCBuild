using EasyDockerFile.Core.Types.System;

namespace EasyDockerFile.Core.Extensions;

public static class MathExtension {
    public static ulong ToLong(this RAMCapacity capacity) {
        return (ulong)capacity;
    }

    public static double ConvertBytesToGB(this ulong capacity) {
        return capacity / Math.Pow(1024, 3);
    }

    public static string ToGBString(this ulong capacity) {
        return $"{ConvertBytesToGB(capacity)}GB";
    }
} 