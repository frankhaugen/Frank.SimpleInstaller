using System.Diagnostics;

namespace Frank.SimpleInstaller;

public class ShortcutTool
{
    public static void CreateShortcut(FileInfo originalFilePath, FileInfo linkPath)
    {
        switch (OperatingSystemDetector.GetOperatingSystem())
        {
            case OperatingSystem.Windows:
                CreateWindowsShortcut(originalFilePath, linkPath);
                break;
            case OperatingSystem.MacOS:
                CreateMacAlias(originalFilePath, linkPath);
                break;
            case OperatingSystem.Linux:
                CreateLinuxSymbolicLink(originalFilePath, linkPath);
                break;
            default:
                throw new NotSupportedException("Unsupported operating system.");
        }
    }

    private static void CreateMacAlias(FileInfo originalFilePath, FileInfo aliasPath)
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
    }

    private static void CreateLinuxSymbolicLink(FileInfo originalFilePath, FileInfo linkPath)
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
    }

    private static void CreateWindowsShortcut(FileInfo originalFilePath, FileInfo shortcutPath)
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
        
        if (process.ExitCode != 0)
        {
            Console.WriteLine($"Failed to create shortcut. Exit code: {process.ExitCode}. Output: {output}");
        }
        else
        {
            Console.WriteLine("Shortcut created successfully.");
        }
    }
}