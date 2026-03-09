using EasyDockerFile.Core.Types.ImageTypes;

namespace EasyDockerFile.Core.Types.DockerTypes;

public class DockerImage(Image selectedImage) 
{
    public string ImageName => GetImageName(selectedImage);

    private static string GetImageName(Image image)
    {
        // Currently this is excessive as full name could be parsed.
        // However, with future maintenance in mind, this is a more robust solution. 
        string baseName = image.FullName.Split(' ')[0].ToLower();
        string version = image.Version.ToLower();

        return $"{baseName}:{version}";
    }
}