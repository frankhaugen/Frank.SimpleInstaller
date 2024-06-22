using System.IO.Compression;

using Frank.SimpleInstaller.Models;

using Spectre.Console;

namespace Frank.SimpleInstaller.Helpers;

public static class InstallationHelper
{
    public static bool Uninstall(string appName)
    {
        var startMenuDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)));
        var startMenuProgramsDirectory = new DirectoryInfo(Path.Combine(startMenuDirectory.FullName, "Programs"));
        
        AnsiConsole.WriteLine($"Removing start menu shortcuts for '{appName}'");
        
        var appFolder = startMenuProgramsDirectory.GetDirectories(appName);
        
        if (appFolder.Length == 0)
        {
            AnsiConsole.WriteLine($"No start menu shortcuts found for '{appName}'");
        }
        else
        {
            foreach (var folder in appFolder)
            {
                folder.Delete(true);
            }
        }

        return true;
    }

    public static bool Install(string appName, Version version)
    {
        return Install(new FileInfo(Path.Combine(AppContext.BaseDirectory, Constants.AppSourceFolderName, $"{appName}.{version}.zip")));
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

        DirectoryInfo installationDirectory = GetInstallationDirectory(metadata);

        DirectoryInfo startMenuDirectory = GetStartMenuDirectory(metadata);

        IEnumerable<ZipArchiveEntry> zipSourceDirectoryEntries = GetZipSourceDirectory(zip);

        FileInfo exexutable = new FileInfo(Path.GetTempFileName());

        foreach (ZipArchiveEntry zipSourceDirectoryEntry in zipSourceDirectoryEntries)
        {
            var sourceEntryPath = zipSourceDirectoryEntry.FullName.Replace(Constants.SourceFolderName, string.Empty).TrimStart('\\');
            string destinationPath = Path.Combine(installationDirectory.FullName, sourceEntryPath);
            AnsiConsole.WriteLine($"Extracting '{zipSourceDirectoryEntry.FullName}' to '{destinationPath}'");
            zipSourceDirectoryEntry.ExtractToFile(destinationPath, true);

            if (zipSourceDirectoryEntry.FullName.EndsWith(".exe"))
            {
                exexutable = new FileInfo(destinationPath);
            }
        }

        FileInfo startMenuShortcut = new FileInfo(Path.Combine(startMenuDirectory.FullName, $"{metadata.Name}.lnk"));

        if (startMenuShortcut.Exists)
        {
            AnsiConsole.WriteLine($"Removing existing shortcut '{startMenuShortcut.FullName}'");
            startMenuShortcut.Delete();
        }

        FileInfo startMenuShortcutTarget = new FileInfo(Path.Combine(installationDirectory.FullName, Constants.SourceFolderName, exexutable.FullName));

        AnsiConsole.WriteLine($"Creating shortcut '{startMenuShortcut.FullName}' to '{startMenuShortcutTarget.FullName}'");

        ShortcutHelper.CreateShortcut(startMenuShortcutTarget, startMenuShortcut);

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

    private static DirectoryInfo GetStartMenuDirectory(InstallationMetadata metadata)
    {
        string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        string companyFolderName = string.IsNullOrWhiteSpace(metadata.Company) ? "" : Path.Combine(startMenuPath, metadata.Company);
        string appFolderPath = Path.Combine(companyFolderName, metadata.SafeName ?? metadata.Name);

        var startMenuDirectory = new DirectoryInfo(startMenuPath);
        var startMenuProgramsDirectory = new DirectoryInfo(Path.Combine(startMenuDirectory.FullName, "Programs"));

        DirectoryInfo? appFolderDirectory;
        
        if (metadata.Company is not null)
        {
            var companyDirectory = new DirectoryInfo(Path.Combine(startMenuProgramsDirectory.FullName, metadata.Company));
            AnsiConsole.WriteLine($"Creating company folder '{companyDirectory.FullName}'");
            companyDirectory.Create();
            appFolderDirectory = companyDirectory;
        }
        else
        {
            appFolderDirectory = startMenuProgramsDirectory;
        }
        
        return appFolderDirectory;
    }


    private static DirectoryInfo GetInstallationDirectory(InstallationMetadata metadata)
    {
        DirectoryInfo programDataDirectory = GetProgramDataDirectory();
        string folderName = metadata.SafeName ?? metadata.Name;
        DirectoryInfo installationDirectory = new DirectoryInfo(Path.Combine(programDataDirectory.FullName, folderName));
        if (!installationDirectory.Exists)
        {
            AnsiConsole.WriteLine($"Creating installation directory '{installationDirectory.FullName}'");
            installationDirectory.Create();
        }

        return installationDirectory;
    }

    private static DirectoryInfo GetProgramDataDirectory()
    {
        DirectoryInfo programDataDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)));
        if (!programDataDirectory.Exists)
        {
            AnsiConsole.WriteLine($"Creating program data directory '{programDataDirectory.FullName}'");
            programDataDirectory.Create();
        }

        return programDataDirectory;
    }
}