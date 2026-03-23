namespace EasyDockerFile.Core.Types.Git;

using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Helpers;
using Octokit;
using Spectre.Console;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Global.Logging;

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
            return CreateRepoClient(gitClient, repoInfo);
        }
        
        // At this point RequiresAuth is guaranteed to be true.
        if (repoInfo.Authentication != null)
        {
            gitClient.Credentials = repoInfo.Authentication;
            repoInfo.UserInfo = await gitClient.User.Current();
            client = CreateRepoClient(gitClient, repoInfo);
        }

        else {
            client = null;
            WriteErrorMessage("The specified repository requires an OAuth Token to access.");
            WriteInformation("Please include an OAuth Token in your command.");
            WriteInformation("Use the --help flag for more information.");
            Environment.Exit(1);
        }
        return client!;
    }


    private static RepoClient CreateRepoClient(GitHubClient client, RepoInfo repoInfo)
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

    public bool RepoContainsFile(string fileName) 
    {   
        // Iterates through each of the tree files in _repoInfo.TreeFiles
        // Returns true if the filepath is exactly the fileName, or if it ends with /fileName.
        return _repoInfo.TreeFiles.Any(f => 
            f.Equals(fileName, StringComparison.OrdinalIgnoreCase) || 
            f.EndsWith("/" + fileName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<int> GetFileCountOfBranch() {
        if (_repoInfo.SelectedBranch == null) {
            WriteErrorMessage("_client._repoInfo.SelectedBranch is not set.", exit: true);
        }

        var treeResponse = await _client.Git.Tree.GetRecursive(
            _repoInfo.RepoUrlObj.Username, 
            _repoInfo.RepoUrlObj.RepoName, 
            _repoInfo.SelectedBranch.Commit.Sha
        );

        _repoInfo.TreeFiles = [.. treeResponse.Tree
            .Where(item => item.Type == TreeType.Blob)
            .Select(item => item.Path)];

        return _repoInfo.TreeFiles.Count();
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
    
    /// <summary>
    ///     Used in UpdateProjectLanguagesAsync()
    /// </summary>
    /// 
    /// <param name="projectLangs">
    ///     A list of RepositoryLanguages returned by the GithubClient object.
    /// </param>
    /// 
    /// <param name="parsedLangs">
    ///     A reference to an List<RepoLanguage> that is to be initialized prior to executing this function.
    /// </param>
    private static void ParseBranchForLanguages(IReadOnlyList<RepositoryLanguage> projectLangs, ref List<RepoLanguage> parsedLangs) 
    {
        foreach(var projectLang in projectLangs) 
        {
            if (projectLang.Name == "C") {
                parsedLangs.Add(RepoLanguage.Native_C);
                continue;
            }

            if (projectLang.Name == "C++") {
                parsedLangs.Add(RepoLanguage.C_Plus_Plus);
                continue;
            }

            if (projectLang.Name == "CMake") {
                parsedLangs.Add(RepoLanguage.CMake);
                continue;
            }

            if (projectLang.Name == "Makefile") {
                parsedLangs.Add(RepoLanguage.Makefile);
                continue;
            }

            if (projectLang.Name == "Meson") {
                parsedLangs.Add(RepoLanguage.Meson);
                continue;
            }
        }
    }

    public async Task UpdateBranchesAsync()
    {
        if (_client.Repository == null) {
            WriteWarningMessage("Unable to locate a repository at the provided uri.");
            WriteErrorMessage("_client.Repository is null in UpdateBranchesAsync()");
            WriteInformation("If you are positive the uri you provided is correct, please run:");
            Console.WriteLine("[COMMAND]: easy-dockerfile --private url/to/repo");
            Environment.Exit(1);
        }
        IReadOnlyList<Branch> branchesObj = [];

        if (_repoInfo == null) {
            WriteWarningMessage("Unable to locate a repository at the provided uri.");
            WriteErrorMessage("_repoInfo is null in UpdateBranchesAsync()", exit: true);
        }

        try {
            _repoInfo.UpdateBranches(
                await _client.Repository.Branch.GetAll(
                    _repoInfo.RepoUrlObj.Username,
                    _repoInfo.RepoUrlObj.RepoName 
                )
            );
        }

        catch (NotFoundException) {
            WriteWarningMessage("Unable to locate branches at the provided repository uri.");
            WriteErrorMessage("Error type: NotFoundException");
            WriteErrorMessage($"Repository uri `{_repoInfo.RepoUrlObj.GetAbsoluteUrlOfRepo()}` was not resolved.");
            WriteInformation("If you are positive the uri you provided is correct, please run:");
            Console.WriteLine("[COMMAND]: easy-dockerfile --private url/to/repo");
            Environment.Exit(1);
        }
        
        var branches = _repoInfo.GetBranches() ?? [];


        var branchNameChoice = InputHelper.AskForInput(
            message: "Please select a branch to use from the list below.", 
            options: branches.Select(branch => branch.Name),
            pageSize: branches.Count < 10 ? branches.Count : 10 // Max out at 10 items per page.
        );

        _repoInfo.SelectedBranch = 
            branches
            .Where(branch => branch.Name == branchNameChoice)
            .FirstOrDefault();
    }

    public async Task UpdateBranchFileCount() {
        _repoInfo.SelectedBranchFileCount = await GetFileCountOfBranch();
    }
    public async Task UpdateProjectLanguagesAsync() 
    {
        var parsedLangs = new List<RepoLanguage>();

        if (_repoInfo?.RepoUrlObj?.Username == null) {
            WriteWarningMessage("Unable to retrieve the contents of the repository at the provided uri.");
            WriteErrorMessage("'_repoInfo.RepoUrlObj.Username' is null in UpdateFileNamesAsync()", exit: true);
        }

        if (_repoInfo?.RepoUrlObj?.RepoName == null) {
            WriteWarningMessage("Unable to the contents of the repository at the provided uri.");
            WriteErrorMessage("Variable '_repoInfo.RepoUrlObj.RepoName' is null in UpdateFileNamesAsync()", exit: true);
        }

        var projectLangs = await _client.Repository.GetAllLanguages(_repoInfo.RepoUrlObj.Username, _repoInfo.RepoUrlObj.RepoName);
        
        ParseBranchForLanguages(projectLangs, ref parsedLangs);

        _repoInfo.ProjectLanguages = parsedLangs;
        // var projectTopics = await _client.Repository.GetAllTopics(_repoInfo.RepoUrlObj.Username, _repoInfo.RepoUrlObj.RepoName);
        
    }

    public void UpdateStatus() {

        if (_repoInfo == null) {
            WriteInformation("_repoInfo is null, skipping _client.UpdateStatus()");
            return;
        }
        bool branchesExist = _repoInfo.GetBranches() != null;

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
            {_repoInfo.GetBranches()?.AsPrettyPrintedBranchString()} 
        -----------------------------------
        - Selected Branch: {_repoInfo.SelectedBranch?.Name ?? "Not Selected"}
        -----------------------------------
        - Selected Branch File Count: {_repoInfo.SelectedBranchFileCount?.ToString() ?? "Not Set"}
        -----------------------------------
        - Languages in Branch:
            {_repoInfo.ProjectLanguages.AsPrettyPrintedLanguageList()}
        -----------------------------------
        - Files in Branch:
            {_repoInfo.TreeFiles.AsPrettyPrintedPathList()}
        """;
    }
}