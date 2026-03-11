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

public class RepoInfo(string repoURL, string[] args)
{
    public RepoUrl RepoUrlObj = RepoUrl.Build(repoURL);
    public RepoStatus Status = args.Contains("--private") ? RepoStatus.Private : RepoStatus.NotSet;
    public Credentials? Authentication = GetTokenFromArgs(args) is string token 
        ? new Credentials(token) 
        : null;
    public IEnumerable<string> BranchNames { get; set; } = [];
    public bool IsPrivate => Status == RepoStatus.Private;
    public bool IsValid => Status != RepoStatus.NotFound && Status != RepoStatus.NotSet;
    public bool RequiresAuth => Authentication != null && this.GetOAuthToken() != null;
    

    private static string? GetTokenFromArgs(string[] args) {
        return args
              .Where(arg => arg.StartsWith("--token="))
              .Select(arg => arg.Replace("--token=", ""))
              .FirstOrDefault();
    }
}



