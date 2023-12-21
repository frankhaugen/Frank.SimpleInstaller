using System.IO.Compression;

namespace Frank.SimpleInstaller;

public class InstallationService : IInstallationService
{
    public bool Install(FileInfo installerFile)
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
            Console.WriteLine($"Extracting '{zipSourceDirectoryEntry.FullName}' to '{destinationPath}'");
            zipSourceDirectoryEntry.ExtractToFile(destinationPath, true);

            if (zipSourceDirectoryEntry.FullName.EndsWith(".exe"))
            {
                exexutable = new FileInfo(destinationPath);
            }
        }

        FileInfo startMenuShortcut = new FileInfo(Path.Combine(startMenuDirectory.FullName, $"{metadata.Name}.lnk"));

        if (startMenuShortcut.Exists)
        {
            Console.WriteLine($"Removing existing shortcut '{startMenuShortcut.FullName}'");
            startMenuShortcut.Delete();
        }

        FileInfo startMenuShortcutTarget = new FileInfo(Path.Combine(installationDirectory.FullName, Constants.SourceFolderName, exexutable.FullName));

        Console.WriteLine($"Creating shortcut '{startMenuShortcut.FullName}' to '{startMenuShortcutTarget.FullName}'");

        ShortcutTool.CreateShortcut(startMenuShortcutTarget, startMenuShortcut);

        return true;
    }

    private IEnumerable<ZipArchiveEntry> GetZipSourceDirectory(ZipArchive zipArchive)
    {
        List<ZipArchiveEntry>? sourceEntry = zipArchive.Entries.Where(x => x.FullName.StartsWith(Constants.SourceFolderName)).ToList();
        if (sourceEntry is null || !sourceEntry.Any())
        {
            throw new OperationCanceledException($"The zip file does not contain a '{Constants.SourceFolderName}' folder.");
        }

        return sourceEntry;
    }

    private DirectoryInfo GetStartMenuDirectory(InstallationMetadata metadata)
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
            Console.WriteLine($"Creating company folder '{companyDirectory.FullName}'");
            companyDirectory.Create();
            appFolderDirectory = companyDirectory;
        }
        else
        {
            appFolderDirectory = startMenuProgramsDirectory;
        }
        
        return appFolderDirectory;
    }


    private DirectoryInfo GetInstallationDirectory(InstallationMetadata metadata)
    {
        DirectoryInfo programDataDirectory = GetProgramDataDirectory();
        string folderName = metadata.SafeName ?? metadata.Name;
        DirectoryInfo installationDirectory = new DirectoryInfo(Path.Combine(programDataDirectory.FullName, folderName));
        if (!installationDirectory.Exists)
        {
            Console.WriteLine($"Creating installation directory '{installationDirectory.FullName}'");
            installationDirectory.Create();
        }

        return installationDirectory;
    }

    private DirectoryInfo GetProgramDataDirectory()
    {
        DirectoryInfo programDataDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)));
        if (!programDataDirectory.Exists)
        {
            Console.WriteLine($"Creating program data directory '{programDataDirectory.FullName}'");
            programDataDirectory.Create();
        }

        return programDataDirectory;
    }

    public bool Install(string appName, string version)
    {
        return Install(appName, Version.Parse(version));
    }

    public bool Install(string appName, Version version)
    {
        return Install(new FileInfo(Path.Combine(AppContext.BaseDirectory, Constants.AppSourceFolderName, $"{appName}.{version}.zip")));
    }
}