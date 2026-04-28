namespace EasyDockerFile.Core.Helpers;

using static Global.Logging;

public static class DirectoryHelper
{   
    private readonly static string baseAppDataDirectory = DetermineUserAppDataDirectory();
    private readonly static string appDataSubDir = Path.Combine(baseAppDataDirectory, "SimpleCBuild");
    public static void CreateAppDirectories() 
    {
        var directoryNames = new string[] { "Cache", "Config", "Data", "Output" };
        try
        {
            foreach (var directoryName in directoryNames)
            {
                var absoluteDirectoryPath = Path.Combine(appDataSubDir, directoryName);
                
                if (!Directory.Exists(absoluteDirectoryPath)) 
                {
                    Directory.CreateDirectory(absoluteDirectoryPath);
                }
            }
        }
        catch (Exception ex) {
            WriteWarningMessage("Unable to create the application data subdirectory for SimpleCBuild.");
            WriteErrorMessage(ex.Message, exitCode: 1, exit: true);
        }
        WriteSuccessMessage("Created application data subdirectory for SimpleCBuild.");
        WriteInformation($"Path: {appDataSubDir}");
    }

    private static string DetermineUserAppDataDirectory() 
    {
        var AppDataDir = string.Empty;

        try 
        {
            AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (!Directory.Exists(AppDataDir)) 
            {
                WriteWarningMessage("Unable to locate the current user's application data directory.");
                WriteErrorMessage(
                    $"The directory is expected to be at: '{AppDataDir}' but was not found.",
                    exit: true,
                    exitCode: 1
                );
            }
        }

        catch (Exception ex) 
        {
            WriteWarningMessage("Unable to locate the current user's application data directory.");
            WriteErrorMessage(
                ex.Message,
                exit: true,
                exitCode: 1
            );    
        }

        return AppDataDir;
    }

    /// <summary>
    /// Returns the absolute path to the current users AppData directory.
    /// </summary>
    public static string GetBaseAppDataDirectory() {
        return baseAppDataDirectory;
    }
    

    /// <summary>
    /// Returns the absolute path of the SimpleCBuild's Content subdirectory within the current user's Application Directory.
    /// </summary>
    public static string GetSCBAppDataDirectory() {
        return appDataSubDir;
    }


    /// <summary>
    /// 
    /// </summary>
    // public static string Get
}