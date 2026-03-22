namespace EasyDockerFile.Core.Types.Git;

using static Global.Logging;

public class RepoUrl 
{
    public string? Protocol;
    public string? Domain;
    public string? Username;
    public string? RepoName;

    public override string ToString()
    {
        return $"""
        Protocol: {Protocol}
        Domain: {Domain}
        Username: {Username}
        RepoName: {RepoName}
        """;
    }


    public string GetAbsoluteUrlOfRepo() {
        var fields = new string?[] {Protocol, Domain, Username, RepoName};
        if (fields.Any(field => field == null)) {
            throw new Exception("One of the members of the RepoUrl object is null.");
        }
        return $"{Protocol}://{Domain}/{Username}/{RepoName}";
    }

    public string GetAbsoluteUrlOfFileInRepo(string branch, string filePath) {
        var absoluteRepoUrl = GetAbsoluteUrlOfRepo();
        return $"{absoluteRepoUrl}/tree/{branch}/{filePath}";  
    }

    public string GetRepoString() {
        return $"{Username}/{RepoName}";
    }

    public static RepoUrl Build(string? repoURL) 
    {
        if (repoURL == null) {
            WriteWarningMessage("Unable to build System.Uri object");
            WriteErrorMessage("repoURL is null in RepoUrl.Build(...)", exit: true);
        }

        Uri? uri = null;
        try {
            uri = new Uri(repoURL);
        }
        catch (Exception ex) {
            WriteWarningMessage("Unable to build System.Uri object");
            WriteErrorMessage($"Error type: {ex.GetType()}");
            WriteErrorMessage(ex.Message, exit: true);
        }

        var isGithubUri = uri.Host.ToLower().Equals("github.com") || uri.Host.ToLower().Equals("www.github.com");
        
        if (isGithubUri) {
            return BuildUrlObjectForGithubRepo(uri);
        }

        throw new ArgumentNullException(message: "Invalid to deteermine provider in RepoUrl.Build", null);
    }

    private static RepoUrl BuildUrlObjectForGithubRepo(Uri uri)
    {
        var isValidDomain = uri.Host.ToLower().Equals("github.com") || uri.Host.ToLower().Equals("www.github.com");

        if (!isValidDomain) {
            WriteWarningMessage("Invalid 'url' param passed to RepoUrl.Build(url, GitProvider.Github)");
            WriteErrorMessage("Parameter `url` must start point to github.com/user/repo or www.github.com/user/repo", exit: true);
        }

        if (uri.Segments.Length != 3) {
            WriteWarningMessage("Invalid 'url' param passed to RepoUrl.Build(url, GitProvider.Github)");
            WriteErrorMessage("uri.Segments.Length was not equal to 3.", exit: true);
        }

        return new RepoUrl(){
            Protocol = uri.Scheme,
            Domain = uri.Host.ToLower(),
            Username = uri.Segments[1].Replace('/', ' ').Trim(),
            RepoName = uri.Segments[2].Replace('/', ' ').Trim()
        };
        
    }
}