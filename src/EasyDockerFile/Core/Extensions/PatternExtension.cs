namespace EasyDockerFile.Core.Extensions;

public static class PatternExtension 
{
    public static bool FileInRootDirectory(this IEnumerable<string> files, string fileName) {
        return files.Contains(fileName);
    }

    public static bool IsFound(this IEnumerable<string> files, string pattern) 
    {
        var searchRootDirOnly = !pattern.Contains('*');
        var fileExtensionOnly = pattern.StartsWith('.');


        if (searchRootDirOnly) {
            return files.Any(file => file.Equals(pattern));
        }
        
        IEnumerable<string> potentialMatches;
        
        if (fileExtensionOnly) {
            potentialMatches = files.Where(file => file.EndsWith(pattern));
        } else {
            potentialMatches = files.Where(file => file.Contains(pattern[1..]));
        }

        return potentialMatches.Any();
    }
    
    public static bool AnyAreFound(this IEnumerable<string> files, string[] patterns) 
    {
        foreach (var pattern in patterns) 
        {
            if (files.IsFound(pattern)) {
                return true;
            }
        }
        return false;
    }
}