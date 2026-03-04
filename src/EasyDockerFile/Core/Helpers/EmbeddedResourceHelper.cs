using System.Reflection;
using static EasyDockerFile.Core.Common.Constants;

namespace EasyDockerFile.Core.Helpers
{
    public class EmbeddedResourceHelper 
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

        private static readonly TaskStatus[] TaskStates = [
            TaskStatus.Canceled,
            TaskStatus.Faulted,
            TaskStatus.RanToCompletion,
        ];

        public static Stream GetEmbeddedResource(string resourceName, string resourcePattern) 
        {
            Stream? resourceStream = null;

            try
            {
                resourceStream = assembly.GetManifestResourceStream(resourcePattern);

                if (resourceStream == null) 
                {
                    Console.WriteLine($"[WARNING]: Unable to retrieve the contents of '{resourceName}'");
                    Console.WriteLine($"[ERROR]: Pattern '{resourcePattern}' returned a null resourceStream.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING]: Unable to retrieve the contents of '{resourceName}'");
                Console.WriteLine($"[ERROR]: {ex.Message}");
                Environment.Exit(1);
            }

            return resourceStream;
        }

        private static void HandleOptionalChecks(string resourceName, Dictionary<string, bool[]>? optionalChecks = null) 
        {
            // Returns IEnumerable<string>?
            var failedChecks = optionalChecks?
                                .Where(pair => pair.Value.Any(check => !check))
                                .Select(pair => pair.Key);

            if (failedChecks != null && failedChecks.Any()) 
            {
                var failedChecksText = string.Join(NLC, failedChecks);

                Console.WriteLine($"[WARNING]: Unable to write embedded resource '{resourceName}' to disk due to a failed check.");
                Console.WriteLine($"[ERROR]: The following conditionals returned false: {failedChecksText}");
                Environment.Exit(1);
            }

        }

        private static async Task HandleSuccessFunction(Task? SuccessFunction = null)
        {
            // Since this is optional, it may not always be passed.
            if (SuccessFunction != null) 
            {
                SuccessFunction.Start();

                // While the SuccessFunction is still actively running, async sleep every second until completion.
                while (TaskStates.All(status => SuccessFunction.Status != status)) 
                {

                    await SuccessFunction.WaitAsync(
                        new CancellationTokenSource(
                            TimeSpan.FromSeconds(1)
                        ).Token
                    );
                }

            }
        }

        public static async Task WriteEmbeddedResourceToDisk(
            string resourceName, string resourcePattern, string outputPath, 
            Dictionary<string, bool[]>? optionalChecks = null, Task? SuccessFunction = null
        ) 
        {
            Stream stream = GetEmbeddedResource(resourceName, resourcePattern);
            await WriteEmbeddedResourceToDisk(stream, resourceName, outputPath, optionalChecks, SuccessFunction);
            
        }

        public static async Task WriteEmbeddedResourceToDisk(
            Stream stream, string resourceName, string outputPath, 
            Dictionary<string, bool[]>? optionalChecks = null, Task? SuccessFunction = null
        ) 
        {
            // If any checks were provided, and one or more of the checks failed:
            // An error is triggered before the success function can execute.
            HandleOptionalChecks(resourceName, optionalChecks);

            await HandleSuccessFunction(SuccessFunction);

            await ReadFromStreamAndWriteToPath(stream, resourceName, outputPath);
            
        }

        private static async Task ReadFromStreamAndWriteToPath(Stream stream, string resourceName, string outputPath)
        {
            try 
            {

                if (stream.Length == 0) {
                    Console.WriteLine($"[WARNING]: Unable to write embedded resource '{resourceName}' to disk.");
                    Console.WriteLine($"[ERROR]: The stream object associated with '{resourceName}' has a length of 0.");
                    Environment.Exit(1);
                }
                
                
                var bufferArray = new byte[stream.Length];

                var bytesLeftToRead = bufferArray.Length;
                
                while (stream.Position < stream.Length) {

                    // Using 1MB chunk size or the remaining buffer is less than 1MB in size (1024 bytes).
                    var chunkSize = stream.Length - stream.Position > 1024 ? 1024 : (int)(stream.Length - stream.Position);

                    // Using chunked reading because the associated performance gains
                    stream.ReadExactly(bufferArray, (int)stream.Position, chunkSize);
                    
                    // Reducing the number of remaining bytes to read.
                    bytesLeftToRead -= chunkSize;

                }

                // Writing the contents to outputPath
                await File.WriteAllBytesAsync(outputPath, bufferArray);
            }

            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}