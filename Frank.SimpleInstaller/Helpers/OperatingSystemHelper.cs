using System.Runtime.InteropServices;

using OperatingSystem = Frank.SimpleInstaller.Models.OperatingSystem;

namespace Frank.SimpleInstaller.Helpers;

public static class OperatingSystemHelper
{
    public static OperatingSystem GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OperatingSystem.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OperatingSystem.MacOS;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OperatingSystem.Linux;
        }

        return OperatingSystem.Unknown;
    }
}