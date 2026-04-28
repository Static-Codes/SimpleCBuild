using System.Text;
using DockerFileSharp.Common;
using EasyDockerFile.Core.Common.Commands;
using EasyDockerFile.Core.Types.Git;
using Global.Build;
using static Global.Constants;
using static Global.Logging;

namespace EasyDockerFile.Core.Types.Build.Base;

public class BuildConfig 
{
    public MainMenuSettings? Settings { get; set; }
    public RepoInfo? RepoInfoObj { get; set; }
    public IEnumerable<BuildSystemInfo> BuildSystemsInfo { get; set; } = [];
    public Toolchain? Toolchain { get; set; }
    public DockerImage? SelectedDockerImage { get; set; }
    public bool IsValid() 
    {
        var properties = typeof(BuildConfig).GetProperties(_publicInstanceFlag);
        
        foreach (var prop in properties) 
        {
            if (prop.GetValue(this) == null) {
                WriteWarningMessage("Invalid build config provided.");
                WriteErrorMessage($"{prop.Name} is null in IsValid()");
                return false;
            }
        }
        return true;
    }

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

            else {
                stringBuilder.AppendLine($"{field.Name}");
                stringBuilder.AppendLine($"\t{field}");
            }
        }
        return stringBuilder.ToString();
    }
}