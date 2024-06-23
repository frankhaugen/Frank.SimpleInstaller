using System.IO.Compression;

using Frank.SimpleInstaller.Models;

namespace Frank.SimpleInstaller.Helpers;

public static class InstallationHelper
{
    public static bool Uninstall(InstallationMetadata metadata)
    {
        var installationDirectory = OperatingSystemHelper.GetInstallationDirectory(metadata);
        var startMenuDirectory = OperatingSystemHelper.GetStartMenuApplicationDirectory(metadata, false);
        
        if (startMenuDirectory.Exists)
        {
            startMenuDirectory.Delete(true);
        }
        
        if (installationDirectory.Exists)
        {
            installationDirectory.Delete(true);
        }
        
        return true;
    }
    
    public static bool Install(FileInfo installerFile)
    {
        using ZipArchive zip = ZipFile.Open(installerFile.FullName, ZipArchiveMode.Read);

        ZipArchiveEntry? metadataEntry = zip.GetEntry(Constants.MetadataFilename);
        if (metadataEntry is null)
        {
            return false;
        }

        using Stream metadataStream = metadataEntry.Open();
        InstallationMetadata? metadata = InstallationMetadata.Load(metadataStream);

        if (metadata is null)
        {
            return false;
        }

        DirectoryInfo installationDirectory = OperatingSystemHelper.GetInstallationDirectory(metadata);
        
        var metadataFile = new FileInfo(Path.Combine(installationDirectory.FullName, Constants.MetadataFilename));
        metadataEntry.ExtractToFile(metadataFile.FullName, true);

        DirectoryInfo startMenuDirectory = OperatingSystemHelper.GetStartMenuApplicationDirectory(metadata);

        IEnumerable<ZipArchiveEntry> zipSourceDirectoryEntries = GetZipSourceDirectory(zip);

        FileInfo exexutable = new FileInfo(Path.Combine(installationDirectory.FullName, metadata.ExecutableName));

        foreach (ZipArchiveEntry zipSourceDirectoryEntry in zipSourceDirectoryEntries)
        {
            var sourceEntryPath = zipSourceDirectoryEntry.FullName.Replace(Constants.SourceFolderName, string.Empty).TrimStart('\\').TrimStart('/');
            string destinationPath = Path.Combine(installationDirectory.FullName, sourceEntryPath);

            if (destinationPath == installationDirectory.FullName)
                continue;
            
            zipSourceDirectoryEntry.ExtractToFile(destinationPath, true);
        }

        FileInfo startMenuShortcut = new FileInfo(Path.Combine(startMenuDirectory.FullName, $"{metadata.Name}.lnk"));

        if (startMenuShortcut.Exists)
        {
            startMenuShortcut.Delete();
        }

        ShortcutHelper.CreateShortcut(exexutable, startMenuShortcut);

        return true;
    }

    private static IEnumerable<ZipArchiveEntry> GetZipSourceDirectory(ZipArchive zipArchive)
    {
        List<ZipArchiveEntry>? sourceEntry = zipArchive.Entries.Where(x => x.FullName.StartsWith(Constants.SourceFolderName)).ToList();
        if (sourceEntry is null || !sourceEntry.Any())
        {
            throw new OperationCanceledException($"The zip file does not contain a '{Constants.SourceFolderName}' folder.");
        }

        return sourceEntry;
    }

    public static Dictionary<string, DirectoryInfo> GetInstalledApplications()
    {
        var commonApplicationDataDirectory = OperatingSystemHelper.GetCommonApplicationDataDirectory();
        var installationDirectories = commonApplicationDataDirectory.GetDirectories();
        var installedApplications = new Dictionary<string, DirectoryInfo>();

        foreach (var installationDirectory in installationDirectories)
        {
            var metadataFile = new FileInfo(Path.Combine(installationDirectory.FullName, Constants.MetadataFilename));
            if (metadataFile.Exists)
            {
                using var fileStream = metadataFile.OpenRead();
                var metadata = InstallationMetadata.Load(fileStream);
                fileStream.Close();
                if (metadata is not null)
                {
                    installedApplications.Add(metadata.Name, installationDirectory);
                }
            }
        }

        return installedApplications;
    }
}