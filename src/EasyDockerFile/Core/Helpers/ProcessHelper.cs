using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static Global.Logging;

namespace EasyDockerFile.Core.Helpers;



public static class ProcessHelper 
{
    /// <summary>
    /// Reuses the process object referenced in the call to this function.
    /// <br/> Creates a new Process object using the provided ProcessStartInformation, and assigns it to the above reference.
    /// </summary>
    /// 
    /// <param name="processStartInfo">
    /// A ProcessStartInfo object to be used in the reassignment of the process object.
    /// </param>
    /// 
    /// <param name="process">
    /// A Process? object that will have it's value reassigned to the newly created Process object.
    /// </param>
    public static void ReassignAndRunProcess(ProcessStartInfo processStartInfo, [NotNull] ref Process? process) 
    {
        process = new Process() { StartInfo = processStartInfo };

        process.Start();
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (e.Data != null) {
                Console.WriteLine(e.Data);
            }
        });

        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (e.Data != null) {
                WriteErrorMessage(e.Data);
            }
        });
        process.WaitForExit();
    }
}