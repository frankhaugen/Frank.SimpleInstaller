using System.IO.Compression;

namespace Frank.SimpleInstaller;

public class PackingService : IPackingService
{
    public FileInfo Pack(DirectoryInfo sourceDirectory, DirectoryInfo appsDirectory, InstallationMetadata metadata)
    {
        var appName = metadata.SafeName ?? metadata.Name;
        var packageFileName = $"{appName}.{metadata.Version.ToString()}.zip";
        var zipFile = new FileInfo(Path.Combine(appsDirectory.FullName, packageFileName));

        if (zipFile.Exists)
        {
            var action = ConsoleMenuFactory.PromptForSelection($"A package with the name '{packageFileName}' already exists. Choose an action:", new List<string> { "Overwrite", "Cancel" });
            if (action == "Cancel")
                return zipFile;
            zipFile.Delete();
        }

        using var zipArchive = ZipFile.Open(zipFile.FullName, ZipArchiveMode.Create);

        // Add files to the zip
        foreach (var file in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            var relativeFileName = file.FullName.Replace(sourceDirectory.FullName, string.Empty).TrimStart('\\');
            var zipEntryPath = Path.Combine(Constants.SourceFolderName, relativeFileName);
            zipArchive.CreateEntryFromFile(file.FullName, zipEntryPath);
        }

        // Create and add metadata
        AddMetadataToZip(metadata, zipArchive);
        return zipFile;
    }

    private static void AddMetadataToZip(InstallationMetadata metadata, ZipArchive zipArchive)
    {
        var metadataEntry = zipArchive.CreateEntry("metadata.json");
        using var entryStream = metadataEntry.Open();
        using var streamWriter = new StreamWriter(entryStream);
        streamWriter.Write(metadata.ToString());
    }
}