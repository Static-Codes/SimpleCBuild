using System.Text;
using static Global.Constants;

namespace EasyDockerFile.Core.Types.Build.Meson;
public class MesonProjectBlock
{
    public string? ProjectName;
    public string[]? ProjectLanguages;
    public string[]? ProjectLicenses;
    public string? ProjectVersion;
    public string? MesonVersion;
    public string[]? DefaultOptions;

    public override string ToString()
    {
        var fields = GetType().GetFields(_publicInstanceFlag);
        var stringBuilder = new StringBuilder();
        foreach (var field in fields) 
        {
            if (field.FieldType == typeof(string)) {
                stringBuilder.AppendLine($"{field.Name}: {field.GetValue(this)}");
            }

            if (field.FieldType == typeof(string[])) {
                stringBuilder.AppendLine($"{field.Name}");
                var arrayMembersObj = field.GetValue(this);

                ArgumentNullException.ThrowIfNull(arrayMembersObj, nameof(arrayMembersObj));

                var arrayMembers = (string[])arrayMembersObj;

                foreach (string member in arrayMembers) {
                    stringBuilder.AppendLine($"\t- {member}");
                }
            }
        }
        return stringBuilder.ToString();
    }

}