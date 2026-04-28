using System.Text;
using static Global.Constants;

namespace EasyDockerFile.Core.Types.System;

public struct RAMKit 
{
    public RAMStick[] Sticks { get; set; }
    public int LowestStickSpeed { get; set; }

    // Currently unused due to Linux incompatibility (see: https://github.com/Jinjinov/Hardware.Info/issues/98)
    private bool SameCapacity { get; set; } 
    private bool SameSpeed { get; set; }


    public override string ToString()
    {
        var properties = typeof(RAMKit).GetProperties(_publicInstanceFlag);
        var stringBuilder = new StringBuilder();
        
        foreach (var prop in properties) 
        {
            var value = prop.GetValue(this) ?? "N/A";

            if (value.GetType() == typeof(RAMStick[])) 
            {
                foreach (var stick in (RAMStick[])value) {
                    stringBuilder.AppendLine($"Stick Index: {stick.Index}");
                    stringBuilder.AppendLine($"Capacity: {stick.Capacity}");
                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendLine($"{prop.Name}: {value}");   
            } else {
                stringBuilder.AppendLine($"{prop.Name}: {value}");
            }
        }
        
        stringBuilder.AppendLine();
        return stringBuilder.ToString();
    }
}

