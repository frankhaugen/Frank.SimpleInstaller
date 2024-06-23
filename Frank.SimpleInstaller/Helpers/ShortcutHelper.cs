using System.Diagnostics;

using OperatingSystem = Frank.SimpleInstaller.Models.OperatingSystem;

namespace Frank.SimpleInstaller.Helpers;

public static class ShortcutHelper
{
    public static bool CreateShortcut(FileInfo originalFilePath, FileInfo linkPath) =>
        OperatingSystemHelper.GetOperatingSystem() switch
        {
            OperatingSystem.Windows => CreateWindowsShortcut(originalFilePath, linkPath),
            OperatingSystem.MacOS => CreateMacAlias(originalFilePath, linkPath),
            OperatingSystem.Linux => CreateLinuxSymbolicLink(originalFilePath, linkPath),
            OperatingSystem.Unknown => throw new PlatformNotSupportedException("Creating shortcuts is not supported on this platform"),
            _ => throw new PlatformNotSupportedException("Creating shortcuts is not supported on this platform")
        };

    private static bool CreateMacAlias(FileInfo originalFilePath, FileInfo aliasPath)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = $"-c \"ln -s '{originalFilePath.FullName}' '{aliasPath.FullName}'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
        
        return process.ExitCode == 0;
    }

    private static bool CreateLinuxSymbolicLink(FileInfo originalFilePath, FileInfo linkPath)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = $"-c \"ln -s '{originalFilePath.FullName}' '{linkPath.FullName}'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
        
        return process.ExitCode == 0;
    }

    private static bool CreateWindowsShortcut(FileInfo originalFilePath, FileInfo shortcutPath)
    {
        // string shortcutName = Path.GetFileNameWithoutExtension(originalFilePath.FullName);

        string script = $"""
                         $WshShell = New-Object -ComObject WScript.Shell
                         $Shortcut = $WshShell.CreateShortcut('{shortcutPath}')
                         $Shortcut.TargetPath = '{originalFilePath}'
                         $Shortcut.Save()
                         """;

        ProcessStartInfo startInfo = new()
        {
            FileName = "powershell.exe",
            Arguments = $"-Command {script}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new() { StartInfo = startInfo };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        return process.ExitCode == 0;
    }
}