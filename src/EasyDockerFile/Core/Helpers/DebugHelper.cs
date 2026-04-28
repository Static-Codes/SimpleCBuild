using EasyDockerFile.Core.API.PackageSearch.Mappers;
using EasyDockerFile.Core.Extensions;

namespace EasyDockerFile.Core.Helpers;


public class DebugHelper 
{

    public static Dictionary<string, bool> RunTests(PackageMapping[] mappings) 
    {
        return new Dictionary<string, bool>() {
            { "TestMappings", TestMappings(mappings) },
        };
    }
    
    /// <summary> 
    /// Tests the mapping functionality for Debian packages, returns false if the tests fails, otherwise true.
    /// </summary>
    public static bool TestMappings(PackageMapping[] mappings) 
    { 
        if (mappings.Length == 0) {
            Console.WriteLine("No mappings found.");
            return false;
        }

        foreach (var mapping in mappings.Reverse().Take(20)) 
        {
            Console.WriteLine("\n--- Mapping Details ---");
            Console.WriteLine();

            if (mapping.Paths.Any())
            {
                Console.WriteLine("Paths:");
                Console.WriteLine("\t" + string.Join(Environment.NewLine + "\t", mapping.Paths));
                Console.WriteLine();
            }

            if (mapping.ParsedPackageList.Count != 0)
            {
                var packageLines = 
                    mapping.ParsedPackageList
                    .Select(p => $"Name: {p.Name} | Type: {p.Type}");
                    
                Console.WriteLine("Package Info:");
                Console.WriteLine("\t" + string.Join(Environment.NewLine + "\t", packageLines));
            }

            else
            {
                Console.WriteLine("Package Info: [Unable To Parse]");
            }

            Console.WriteLine();
        }


        var packages = mappings.GetPackagesByType(EasyDockerFile.Core.Types.PackageType.Unknown);
        Console.WriteLine(packages.Length);

        foreach (var package in packages.Take(200)) {
            Console.WriteLine(package.Name);
        }

        return true;
    }
}