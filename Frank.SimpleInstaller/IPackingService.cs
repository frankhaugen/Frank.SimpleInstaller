using System.IO.Compression;

namespace Frank.SimpleInstaller;

public interface IPackingService
{
    FileInfo Pack(DirectoryInfo sourceDirectory, DirectoryInfo appsDirectory, InstallationMetadata metadata);
}