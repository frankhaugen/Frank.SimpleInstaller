using System.Runtime.InteropServices;

using Frank.SimpleInstaller.Models;

using OperatingSystem = Frank.SimpleInstaller.Models.OperatingSystem;

namespace Frank.SimpleInstaller.Helpers;

public static class OperatingSystemHelper
{
    public static OperatingSystem GetOperatingSystem() => 
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? OperatingSystem.Windows 
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) 
                ? OperatingSystem.MacOS 
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) 
                    ? OperatingSystem.Linux 
                    : OperatingSystem.Unknown;

    public static DirectoryInfo GetCommonApplicationDataDirectory() =>
        GetOperatingSystem() switch
        {
            OperatingSystem.Windows => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)),
            OperatingSystem.Linux or OperatingSystem.MacOS => new DirectoryInfo("/usr/share"),
            OperatingSystem.Unknown => throw new PlatformNotSupportedException("Unsupported platform"),
            _ => throw new PlatformNotSupportedException("Unsupported platform")
        };

    public static DirectoryInfo GetInstallationDirectory(InstallationMetadata metadata, bool createIfNotExists = true)
    {
        var directory = new DirectoryInfo(Path.Combine(GetCommonApplicationDataDirectory().FullName, metadata.SafeName ?? metadata.Name));
        if (createIfNotExists && !directory.Exists) directory.Create();
        return directory;
    }

    public static DirectoryInfo GetStartMenuApplicationDirectory(InstallationMetadata metadata, bool createIfNotExists = true)
    {
        DirectoryInfo appDirectory = new(Path.Combine(GetStartMenuProgramsDirectory().FullName, metadata.SafeName ?? metadata.Name));
        if (createIfNotExists && !appDirectory.Exists) appDirectory.Create();
        return appDirectory;
    }

    public static DirectoryInfo GetStartMenuDirectory() =>
        GetOperatingSystem() switch
        {
            OperatingSystem.Windows => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)),
            OperatingSystem.Linux => new DirectoryInfo("/usr/share/applications"),
            OperatingSystem.MacOS => new DirectoryInfo("/Applications"),
            OperatingSystem.Unknown => throw new PlatformNotSupportedException("Start menu is not supported on this platform"),
            _ => throw new PlatformNotSupportedException("Start menu is not supported on this platform")
        };

    public static DirectoryInfo GetStartMenuProgramsDirectory() =>
        GetOperatingSystem() switch
        {
            OperatingSystem.Windows => new DirectoryInfo(Path.Combine(GetStartMenuDirectory().FullName, "Programs")),
            OperatingSystem.Linux => new DirectoryInfo("/usr/share/applications"),
            OperatingSystem.MacOS => new DirectoryInfo("/Applications"),
            OperatingSystem.Unknown => throw new PlatformNotSupportedException("Start menu is not supported on this platform"),
            _ => throw new PlatformNotSupportedException("Start menu is not supported on this platform")
        };

    public static DirectoryInfo GetInstallationDirectory(string appName) 
        => new(Path.Combine(GetCommonApplicationDataDirectory().FullName, appName));
}