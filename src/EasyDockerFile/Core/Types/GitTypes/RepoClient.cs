namespace EasyDockerFile.Core.Types.GitTypes;

using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Helpers;
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

    private async Task<IEnumerable<string>> GetFlattenedFileList(string owner, string repoName, string branchName)
    {
        var files = new List<string>();
        await GetFilesRecursivelyAsync(owner, repoName, files, branchName);
        return files;
    }

    private async Task GetFilesRecursivelyAsync(string owner, string repoName, List<string> files, string branchName, string path = ".") 
    {   
        try 
        {
            var contents = await _client.Repository.Content.GetAllContentsByRef(owner, repoName, path, branchName);
            
            foreach (var content in contents)
            {
                if (content.Type == ContentType.File) {
                    files.Add(content.Path);
                }

                else if (content.Type == ContentType.Dir) {
                    await GetFilesRecursivelyAsync(owner, repoName, files, branchName, content.Path);
                }
            }
        }

        catch (NotFoundException ex) 
        {
            Console.WriteLine($"[ERROR]: Branch not found: {ex.Message}");
            Environment.Exit(1);
        }
        
        catch (Exception ex) {
            var eMessage = Markup.Escape($"[ERROR]: {ex.Message}.");
            AnsiConsole.MarkupLine($"[red]{eMessage}[/]");
            
            if (ex.Message.StartsWith("API rate limit exceeded")) {
                Console.WriteLine("[INFO]: https://docs.github.com/en/rest/using-the-rest-api/rate-limits-for-the-rest-api");
            }
            
            Console.WriteLine("[INFO]: Use the --help flag for more information.");
            Environment.Exit(1);
        }
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

        _repoInfo.SelectedBranchName = InputHelper.AskForInput(
            message: "Please select a branch to use from the list below.", 
            options: _repoInfo.BranchNames
        );
    }

    public async Task UpdateFileNamesAsync() {

        if (_repoInfo?.RepoUrlObj?.Username == null) {
            Console.WriteLine("[WARNING]: Unable to retrieve the contents of the repository at the provided uri.");
            Console.WriteLine("[ERROR]: '_repoInfo.RepoUrlObj.Username' is null in UpdateFileNamesAsync()");
            Environment.Exit(1);
        }

        if (_repoInfo?.RepoUrlObj?.RepoName == null) {
            Console.WriteLine("[WARNING]: Unable to the contents of the repository at the provided uri.");
            Console.WriteLine("[ERROR]: Variable '_repoInfo.RepoUrlObj.RepoName' is null in UpdateFileNamesAsync()");
            Environment.Exit(1);
        }

        if (_repoInfo?.SelectedBranchName == null) {
            Console.WriteLine("[WARNING]: Unable to the contents of the repository at the provided uri.");
            Console.WriteLine("[ERROR]: Variable '_repoInfo.SelectedBranchName' is null in UpdateFileNamesAsync()");
            Environment.Exit(1);
        }

        string branchToUse = _repoInfo.SelectedBranchName;

        try 
        {
            var repo = await _client.Repository.Get(
                _repoInfo.RepoUrlObj.Username, 
                _repoInfo.RepoUrlObj.RepoName
            );
            
            // If the user hasn't specified a branch, a fallback to the DefaultBranch is the solution.
            if (string.IsNullOrEmpty(branchToUse)) {
                branchToUse = repo.DefaultBranch;
            }
        }
        catch (NotFoundException) 
        {
            Console.WriteLine("[ERROR]: Branch not found.");
            Environment.Exit(1);
        }

        var filePaths = await GetFlattenedFileList(
            _repoInfo.RepoUrlObj.Username, 
            _repoInfo.RepoUrlObj.RepoName, 
            $"refs/heads/{_repoInfo.SelectedBranchName}");

        if (!filePaths.Any()) {
            Console.WriteLine("[WARNING]: Unable to the contents of the repository at the provided uri.");
            Console.WriteLine("[ERROR]: Variable 'filePaths' is empty in UpdateFileNamesAsync()");
            Environment.Exit(1);
        }

        _repoInfo.FilePaths = filePaths;
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
        - Repo Is Private: {_repoInfo.IsPrivate}
        -----------------------------------
        - Repo Requires Auth: {_repoInfo.RequiresAuth}
        -----------------------------------
        - Url Is Valid: {_repoInfo.IsValid}
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
        - Selected Branch: {_repoInfo.SelectedBranchName ?? "Not Selected"}    
        -----------------------------------
        - Files in Branch:
            {_repoInfo.FilePaths.AsPrettyPrintedPathList()}
        """;
    }
}