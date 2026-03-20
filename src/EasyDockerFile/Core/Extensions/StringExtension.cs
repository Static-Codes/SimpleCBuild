using System.Text;
using EasyDockerFile.Core.Types.Git;
using Octokit;
using static Global.Constants;

namespace EasyDockerFile.Core.Extensions;

public static class StringExtension
{
    public static string AsPrettyPrintedBranchString(this IReadOnlyList<Branch>? branchNames) {
        if (branchNames == null || !branchNames.Any()) {
            return $"- Unable to resolve, please check your OAuth Token above.";
        }

        return $"- {string.Join($"{NLC}    - ", branchNames.Select(branch => branch.Name))}";
    }

    public static string AsPrettyPrintedLanguageList(this IEnumerable<RepoLanguage> repoLanguages)
    {
        if (repoLanguages == null || !repoLanguages.Any()) {
            return "- Unable to resolve, please check your selected branch above.";
        }

        var formattedLanguages = repoLanguages.Select(lang => 
            lang.ToString().Contains('_') 
                ? lang.Sanitize()
                : lang.ToString()
        );
        
        return $"- {string.Join($"{NLC}    - ", formattedLanguages)}";
    }
    public static string AsPrettyPrintedPathList(this IEnumerable<string> filePaths) {
        if (!filePaths.Any()) {
            return $"- Unable to resolve, please check your selected branch above.";
        }

        return $"- {string.Join($"{NLC}    - ", filePaths)}";
    }
    
    public static bool IsUnix(this string SystemName) {
        return MesonUnixSystemNames.Contains(SystemName);
    }

    /// <summary>
    /// Primarily used to convert Enum member names to their string representation.
    /// </summary>
    public static string Sanitize(this object Enum) {
        return Enum.ToString()!.Replace("_", " ");
    }


}