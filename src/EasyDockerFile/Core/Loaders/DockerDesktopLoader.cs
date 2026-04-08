using EasyDockerFile.Core.Extensions;
using EasyDockerFile.Core.Helpers;
using System.Diagnostics;
using static EasyDockerFile.Core.Helpers.ExecutableHelper;
using static Global.Logging;


namespace EasyDockerFile.Core.Loaders;

public static class DockerDesktopLoader 
{
    private static readonly string DockerExecutableName = "com.docker.backend";
    private static readonly string DockerExecutablePath = GetDockerExecutablePath();
    private static bool IsActive { get; set; } = false;
    private static bool IsInstalled { get; set; } = File.Exists(DockerExecutablePath);

    private static void CheckForActiveInstances() 
    {
        if (IsActive) {
            return;
        }

        var activeDockerProcesses = Process.GetProcessesByName(DockerExecutableName);

        if (activeDockerProcesses.Length > 0) 
        {
            IsActive = true;
            return;
        }
    }

    // This is extremely fragile, but I was not aware of an option within the Docker Installer for custom installation paths.
    private static string GetDockerExecutablePath() 
    {
        if (OperatingSystem.IsWindows()) {
            return @"C:\Program Files\Docker\Docker\Docker Desktop.exe";
        }
        return "/opt/docker-desktop/bin/docker-desktop";
    }

    private static ProcessStartInfo GetDockerStartInfo() 
    {
        var psi = new ProcessStartInfo
        {
            CreateNoWindow = true,
            FileName = GetShellExecutable(),
            Arguments = $"{GetShellArg()} \"{GetDockerExecutablePath()}\"",
        };

        if (OperatingSystem.IsWindows()) {
            psi.CreateNewProcessGroup = true;
        }

        else {
            psi.UseShellExecute = true;
        }

        #if DEBUG
            Console.WriteLine($"Executing: {psi.FileName} {psi.Arguments}");
        #endif

        return psi;
    }

    public static void LoadDockerDesktop() 
    {
        if (!IsInstalled) {
            WriteWarningMessage("Unable to locate an installation of Docker Desktop on the current machine.");
            WriteErrorMessage(
                $"SimpleCBuild expects Docker Desktop to be located at: {DockerExecutablePath}", 
                exitCode: 1, 
                exit: true
            );
        }

        CheckForActiveInstances();

        if (IsActive) {
            WriteSuccessMessage("Docker Desktop was already running.");
            return;
        }

        WriteInformation("Starting Docker Desktop.");

        var psi = GetDockerStartInfo();

        Process? process = null;
        
        ProcessHelper.ReassignAndRunProcess(psi, ref process);

        // Accessing the required data prior to the disposal to prevent an ObjectDisposedException.
        var exitCode = process.ExitCode;

        string? standardError;

        try { 
            standardError = process.StandardError.ReadToEnd();
        }

        catch (InvalidOperationException ex) {
            standardError = ex.Message;
        }


        // Disposing of the process object now that it is no longer used;
        process.DisposeSafely();
        process = null; 

        if (exitCode == 0) {
            WriteSuccessMessage("Docker Desktop was started.");
            IsActive = true;
            return;
        }

        WriteWarningMessage("Unable to start Docker Desktop on the current system.");
        WriteErrorMessage(standardError, exitCode: 1, exit: true);
    }
}