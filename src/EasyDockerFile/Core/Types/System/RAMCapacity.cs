namespace EasyDockerFile.Core.Types.System;

// Binary
public enum RAMCapacity : ulong 
{
    _4GB   = 4294967296,
    _8GB   = 8589934592,
    _16GB  = 17179869184,
    _32GB  = 34359738368,
    _64GB  = 68719476736,
    _128GB = 137438953472,
    _256GB = 274877906944,
    _512GB = 549755813888,
    _1TB   = 1099511627776,
    _UNKNOWN_OR_UNSUPPORTED = 1
}