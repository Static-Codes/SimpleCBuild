namespace EasyDockerFile.Core.Types.GitTypes;

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

    public string GetAbsoluteUrl() {
        var fields = new string?[] {Protocol, Domain, Username, RepoName};
        if (fields.Any(field => field == null)) {
            throw new Exception("One of the members of the RepoUrl object is null.");
        }
        return $"{Protocol}://{Domain}/{Username}/{RepoName}";
    }

    public static RepoUrl Build(string? repoURL) 
    {
        if (repoURL == null) {
            Console.WriteLine("[WARNING]: Unable to build System.Uri object");
            Console.WriteLine($"[ERROR]: repoURL is null in RepoUrl.Build(...)");
            Environment.Exit(1);
        }

        Uri? uri = null;
        try {
            uri = new Uri(repoURL);
        }
        catch (Exception ex) {
            Console.WriteLine("[WARNING]: Unable to build System.Uri object");
            Console.WriteLine($"[ERROR TYPE]: {ex.GetType()}");
            Console.WriteLine($"[ERROR]: {ex.Message}");
            Environment.Exit(1);
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
            Console.WriteLine("[WARNING]: Invalid 'url' param passed to RepoUrl.Build(url, GitProvider.Github)");
            Console.WriteLine($"[ERROR]: Parameter `url` must start point to github.com/user/repo or www.github.com/user/repo");
            Environment.Exit(1);
        }

        if (uri.Segments.Length != 3) {
            Console.WriteLine("[WARNING]: Invalid 'url' param passed to RepoUrl.Build(url, GitProvider.Github)");
            Console.WriteLine($"[ERROR]: uri.Segments.Length was not equal to 3.");
            Environment.Exit(1);
        }

        return new RepoUrl(){
            Protocol = uri.Scheme,
            Domain = uri.Host.ToLower(),
            Username = uri.Segments[1].Replace('/', ' ').Trim(),
            RepoName = uri.Segments[2].Replace('/', ' ').Trim()
        };
        
    }
}