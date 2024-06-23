namespace Frank.SimpleInstaller.Helpers;

public static class DirectoryHelper
{
    public static DirectoryInfo GetBaseDirectory()
    {
        var rootDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        return rootDirectory;
    }
}