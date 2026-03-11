namespace EasyDockerFile.Core.API.ToolchainSearch.Git;

using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Types.GitTypes;
using Octokit;
using Spectre.Console;
using System;
using System.Linq;
using System.Threading.Tasks;

public class RepoClient
{
    private readonly GitHubClient _client;
    private readonly RepoInfo _repoInfo;
    private ApiInfo? _apiInfo;

    private RepoClient(GitHubClient client, RepoInfo repoInfo)
    {
        _client = client;
        _repoInfo = repoInfo;
    }

    private static RepoClient RunCreationHelper(GitHubClient client, RepoInfo repoInfo)
    {
        var repoClient = new RepoClient(client, repoInfo);
        
        // Attempting to capture the last api request info, or silently exiting as a null value will be handled externally.
        try {
            repoClient._apiInfo = client.GetLastApiInfo();
        } 
        catch {
            
        }
        return repoClient;
    }
    public static async Task<RepoClient> CreateAsync(RepoInfo repoInfo)
    {
        var gitClient = new GitHubClient(new ProductHeaderValue(repoInfo.RepoUrlObj.RepoName));
        RepoClient? client;

        if (!repoInfo.RequiresAuth) {
            return RunCreationHelper(gitClient, repoInfo);
        }
        
        // At this point RequiresAuth is guaranteed to be true.
        if (repoInfo.Authentication != null)
        {
            gitClient.Credentials = repoInfo.Authentication;
            repoInfo.UserInfo = await gitClient.User.Current();
            client = RunCreationHelper(gitClient, repoInfo);
        }

        else {
            client = null;
            var eMessage = Markup.Escape("[ERROR]: The specified repository requires an OAuth Token to access.");
            AnsiConsole.MarkupLine($"[red]{eMessage}[/]");
            Console.WriteLine("[INFO]: Please include an OAuth Token in your command.");
            Console.WriteLine("[INFO]: Use the --help flag for more information.");
            Environment.Exit(1);
        }
        return client!;
    }

    public (bool IsRateLimited, Tuple<int, int, DateTimeOffset>? RateLimitInfo) GetRateLimitInfo()
    {
        var rateLimit = _apiInfo?.RateLimit;

        if (rateLimit == null)
        {
            return (false, null);
        }

        var rateLimitInfo = Tuple.Create(
            rateLimit.Limit, 
            rateLimit.Remaining, 
            rateLimit.Reset
        );

        return (true, rateLimitInfo);
    }

    public async Task UpdateBranchesAsync()
    {
        if (_client.Repository == null) {
            Console.WriteLine("[WARNING]: Unable to locate a repository at the provided uri.");
            Console.WriteLine("[ERROR]: _client.Repository is null in UpdateBranchesAsync()");
            Console.WriteLine("[INFO]: If you are positive the uri you provided is correct, please run:");
            Console.WriteLine("[COMMAND]: easy-dockerfile --private url/to/repo");
            Environment.Exit(1);
        }
        IReadOnlyList<Branch> branchesObj = [];

        if (_repoInfo == null) {
            Console.WriteLine("[WARNING]: Unable to locate a repository at the provided uri.");
            Console.WriteLine("[ERROR]: _repoInfo is null in UpdateBranchesAsync()");
            Environment.Exit(1);
        }

        try {
            branchesObj = await _client.Repository.Branch.GetAll(
                _repoInfo.RepoUrlObj.Username,
                _repoInfo.RepoUrlObj.RepoName 
            );
        }

        catch (NotFoundException) {
            Console.WriteLine("[WARNING]: Unable to locate branches at the provided repository uri.");
            Console.WriteLine($"[ERROR TYPE]: NotFoundException");
            Console.WriteLine($"[ERROR]: Repository uri `{_repoInfo.RepoUrlObj.GetAbsoluteUrl()}` was not resolved.");
            Console.WriteLine("[INFO]: If you are positive the uri you provided is correct, please run:");
            Console.WriteLine("[COMMAND]: easy-dockerfile --private url/to/repo");
            Environment.Exit(1);
        }

        _repoInfo.BranchNames = branchesObj.Select(branch => branch.Name) ?? [];
    }
    
    public void UpdateStatus() {

        if (_repoInfo == null) {
            Console.WriteLine("[INFO]: _repoInfo is null, skipping _client.UpdateStatus()");
            return;
        }
        bool branchesExist = _repoInfo.BranchNames.Any();

        if (_repoInfo.Status == RepoStatus.NotSet) {
            _repoInfo.Status = branchesExist ? RepoStatus.Public : RepoStatus.NotFound;
        }
    }



    public override string ToString()
    {
        return $"""
        -----------------------------------
        - RepoUrl: 
            - Protocol: {_repoInfo!.RepoUrlObj.Protocol}
            - Domain: {_repoInfo.RepoUrlObj.Domain}
            - Username: {_repoInfo.RepoUrlObj.Username}
            - RepoName: {_repoInfo.RepoUrlObj.RepoName}
        -----------------------------------
        - Status: {_repoInfo.Status}
        -----------------------------------
        - Authentication: 
            - Is Set: {_repoInfo.Authentication != null}
            - OAuth Token: {_repoInfo.Authentication?.GetToken() ?? "Not Set"}
        -----------------------------------
        - User Info:
            - Is Authenticated: {_repoInfo.UserInfo != null}
            - Username: {_repoInfo.UserInfo?.Login ?? "Not Set"}
            - Public Repos: {_repoInfo.UserInfo?.PublicRepos.ToString() ?? "Not Set"}
        -----------------------------------
        - Branches:
            {_repoInfo.BranchNames.AsPrettyPrintedBranchString()}
        -----------------------------------
        - Is Private: {_repoInfo.IsPrivate}
        -----------------------------------
        - Is Valid: {_repoInfo.IsValid}
        -----------------------------------
        - RequiresAuth: {_repoInfo.RequiresAuth}
        """;
    }
}