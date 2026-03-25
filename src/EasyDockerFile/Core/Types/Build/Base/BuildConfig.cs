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
    public IEnumerable<BuildSystemInfo> BuildSystemInfo { get; set; } = [];
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
}