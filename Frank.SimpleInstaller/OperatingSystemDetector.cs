using System.Runtime.InteropServices;

namespace Frank.SimpleInstaller;

public static class OperatingSystemDetector
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