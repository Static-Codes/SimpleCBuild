using System.Diagnostics.CodeAnalysis;
using EasyDockerFile.Core.Common.Commands;
using EasyDockerFile.Core.Extensions;
using Octokit;

namespace EasyDockerFile.Core.Types.GitTypes;

public enum RepoStatus 
{
    Public = 0,
    Private = 1,
    NotFound = 2,
    NotSet = 3,
}

public class RepoInfo(MainMenuSettings settings)
{
    public RepoUrl RepoUrlObj = RepoUrl.Build(settings.RepoLink);
    public RepoStatus Status = settings.PrivateFlagSet ? RepoStatus.Private : RepoStatus.NotSet;
    public Credentials? Authentication = settings.OAuthToken is string token 
        ? new Credentials(token, AuthenticationType.Oauth) 
        : null;
    public User? UserInfo = null;
    public IEnumerable<string> BranchNames { get; set; } = [];
    public bool IsPrivate => Status == RepoStatus.Private;
    public bool IsValid => Status != RepoStatus.NotFound && Status != RepoStatus.NotSet;
    public bool RequiresAuth => Authentication != null && this.GetOAuthToken() != null;
}



