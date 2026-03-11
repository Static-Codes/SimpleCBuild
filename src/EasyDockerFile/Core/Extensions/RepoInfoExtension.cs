using EasyDockerFile.Core.Types.GitTypes;
using Octokit;

namespace EasyDockerFile.Core.Extensions;

public static class RepoInfoExtension 
{
    public static string GetOAuthToken(this RepoInfo repoInfoObj) {
        return repoInfoObj.Authentication?.GetToken() ?? "No token provided";
    }
}