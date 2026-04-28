using EasyDockerFile.Core.API.PackageSearch.Mappers;
using EasyDockerFile.Core.Types;
using EasyDockerFile.Core.Types.System;
using Global.Build;
using static Global.Logging;

namespace EasyDockerFile.Core.Extensions;

public static class MappingExtension 
{
    public static List<Package> GetSystemDependencies(this List<Package> packages) {
        return [.. packages.Where(package => package.IsSystemDependency())];
    }


    public static bool IsSystemDependency(this Package package) => package.Name.StartsWith("/usr/include/");

    public static Package[] GetPackagesByType(this PackageMapping[] mappings, PackageType packageType) {
        return [.. mappings
            .SelectMany(mapping => mapping.ParsedPackageList)
            .Where(map => map.Type.Equals(packageType))
        ];
            
    }

    public static string GetNameOfMember(this RAMCapacity capacity) {
        return capacity.ToString().Replace('_', ' ').Trim();
    }


    private static BuildSystemName? GetBuildSystemName(string Name) {
        return Enum.TryParse<BuildSystemName>(Name, true, out var result) ? result : null;
    }

    
    public static BuildSystemName[] GetBuildSystemNames(this BuildSystemInfo[] buildSystemsInfo)
    {
        var names = new BuildSystemName[buildSystemsInfo.Length];

        for (int i = 0; i < names.Length; i++ ) 
        {
            var potentialName = GetBuildSystemName(buildSystemsInfo[i].Name);
            if (potentialName == null) {
                WriteErrorMessage($"Invalid build system parsed from BuildSystems.xml: '{buildSystemsInfo[i].Name}'");
                return [];
            }
            names[i] = (BuildSystemName)potentialName;
        }

        return names.Length != 0 ? names : [];
    }
}