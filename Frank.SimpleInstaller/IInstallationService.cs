namespace Frank.SimpleInstaller;

public interface IInstallationService
{
    bool Install(FileInfo installerFile);

    bool Install(string appName, string version);

    bool Install(string appName, Version version);
}