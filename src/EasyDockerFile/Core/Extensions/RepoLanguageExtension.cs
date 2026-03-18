using EasyDockerFile.Core.Types.Git;

namespace EasyDockerFile.Core.Extensions;

public static class RepoLanguageExtension 
{
    /// <summary>
    /// Checks if the project contains a supported language.
    /// Returns false if Native_C/C_Plus_Plus are missing OR if the list only contains Not_Set/Other.
    /// </summary>
    public static bool HasValidTargetLanguage(this RepoLanguage[] languages)
    {
        if (languages == null || languages.Length == 0) {
            return false;
        }

        return languages.Any(lang => 
            lang == RepoLanguage.Native_C || 
            lang == RepoLanguage.C_Plus_Plus
        );
    }
}