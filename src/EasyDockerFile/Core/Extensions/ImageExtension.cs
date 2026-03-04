using EasyDockerFile.Core.Types.ImageTypes;

namespace EasyDockerFile.Core.Extensions;

public static class ImageExtension 
{
    public static void ShowFamilyInfo(this ImageFamily imageFamily) 
    {
        Console.WriteLine($"Family: {imageFamily.Name}");
        foreach (var image in imageFamily.Images) {
            Console.WriteLine($"\tVersion: {image.Version}");
            foreach (var architecture in image.SupportedArchitectures){
                Console.WriteLine($"\t\t- Supported Architecture: {architecture}");
            }
        }
    }
}