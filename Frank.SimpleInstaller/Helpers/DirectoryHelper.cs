using System.Text.RegularExpressions;

namespace Frank.SimpleInstaller.Helpers;

public static class DirectoryHelper
{
    public static DirectoryInfo GetBaseDirectory()
    {
        var rootDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        return rootDirectory;
    }
    
    public static List<string> GetApps(DirectoryInfo rootDirectory)
    {
        DirectoryInfo appsDirectory = new DirectoryInfo(Path.Combine(rootDirectory.FullName, Constants.AppSourceFolderName));
        
        if (!appsDirectory.Exists)
            return new List<string>();

        FileInfo[]? zipFiles = appsDirectory.GetFiles("*.zip");

        List<string>? filenames = zipFiles.Select(f => f.Name.Replace(".zip", "")).ToList();

        // Remove version numbers from filenames
        string pattern = @"\.\d+\.\d+\.\d+$";

        // Get distinct app names
        IEnumerable<string>? appNames = filenames.Select(f => Regex.Replace(f, pattern, "")).Distinct();

        // Sort alphabetically and return
        return appNames.Order().ToList();
    }

    public static List<Version> GetVersionsForApp(DirectoryInfo rootDirectory, string s)
    {
        // Remove app name from filenames
        var appsDirectory = new DirectoryInfo(Path.Combine(rootDirectory.FullName, Constants.AppSourceFolderName));
        
        var appFiles = appsDirectory.GetFiles("*.zip").ToList();
        var appNames = appFiles.Where(f => f.Name.StartsWith(s)).ToList();
        var appnames2 = appNames.Select(f => f.Name.Replace(".zip", "")).ToList();
        var appnames3 = appnames2.Select(f => f.Replace(s, "")).Distinct().ToList();
        var appnames4 = appnames3.Select(f => f.TrimStart('.')).Distinct().ToList();
        var versions = appnames4.Select(Version.Parse).OrderDescending().ToList();
        return versions;
    }
}