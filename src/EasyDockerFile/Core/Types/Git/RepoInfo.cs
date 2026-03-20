using EasyDockerFile.Core.Common.Commands;
using EasyDockerFile.Core.Extensions;
using Octokit;
using Spectre.Console;
using static Global.Constants;

namespace EasyDockerFile.Core.Types.Git;

public class RepoInfo(MainMenuSettings settings)
{
    public RepoUrl RepoUrlObj = RepoUrl.Build(settings.RepoLink);
    public RepoStatus Status = settings.PrivateFlagSet ? RepoStatus.Private : RepoStatus.NotSet;
    public Credentials? Authentication = settings.OAuthToken is string token 
        ? new Credentials(token, AuthenticationType.Oauth) 
        : null;
    public User? UserInfo = null;
    public Branch? SelectedBranch { get; set; } = null;
    public int? SelectedBranchFileCount { get; set; } = null;
    public IEnumerable<RepoLanguage> ProjectLanguages { get; set; } = [];
    public IEnumerable<string> FilePaths { get; set; } = [];
    public bool IsPrivate => Status == RepoStatus.Private;
    public bool IsValid => Status != RepoStatus.NotFound && Status != RepoStatus.NotSet;
    public bool RequiresAuth => Authentication != null && this.GetOAuthToken() != null;
    private IReadOnlyList<Branch>? BranchesObj { get; set; } = null;

    public IReadOnlyList<Branch>? GetBranches() => BranchesObj;
    public void UpdateBranches(IReadOnlyList<Branch>? branch)
    {
        if (branch == null) {
            AnsiConsole.Write($"[yellow]{WarningTag}[/] Unable to update RepoInfo.BranchObj");
            AnsiConsole.Write($"[red]{ErrorTag}[/] branch is null in RepoInfo.UpdateBranch()");
            return;
        }
        BranchesObj = branch; 
    }

}



