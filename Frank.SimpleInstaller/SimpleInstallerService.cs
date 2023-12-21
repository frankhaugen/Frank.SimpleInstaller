namespace Frank.SimpleInstaller;

public class SimpleInstallerService : ISimpleInstallerService
{
    private readonly IInstallationService _installationService;
    private readonly IPackingService _packingService;

    public SimpleInstallerService(IInstallationService installationService, IPackingService packingService)
    {
        _installationService = installationService;
        _packingService = packingService;
    }

    public void Run(DirectoryInfo rootDirectory)
    {
        List<string> apps = DirectoryTool.GetApps(rootDirectory); // Method to get the list of available apps

        if (apps.Count == 0)
        {
            HandlePackageCreation(rootDirectory);
        }
        else
        {
            HandleAppSelection(rootDirectory, apps);
        }
    }

    private void HandlePackageCreation(DirectoryInfo rootDirectory)
    {
        DirectoryInfo directoryPath = ConsoleMenuFactory.PromptForDirectoryInput("Enter the path to the directory to create a package:");
        
        Version version = ConsoleMenuFactory.PromptForVersionInput("Enter the version number for the package:");
        string appName = ConsoleMenuFactory.PromptForStringInput("Enter the name of the application to package:");

        string? safeAppName = null;
        if (appName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            safeAppName = ConsoleMenuFactory.PromptForFilenameInput($"The application name '{appName}' contains invalid characters. Enter a safe name for the package:");
        }
        
        var company = ConsoleMenuFactory.PromptForStringOrNullInput("Enter the name of the company that created the application (optional):");
        
        InstallationMetadata metadata = new InstallationMetadata { Name = appName, Version = version, SafeName = safeAppName, Company = company };
        
        var appsDirectory = new DirectoryInfo(Path.Combine(rootDirectory.FullName, Constants.AppSourceFolderName));
        
        if (!appsDirectory.Exists)
            appsDirectory.Create();

        FileInfo result = _packingService.Pack(directoryPath, appsDirectory, metadata);
        Console.WriteLine($"Package created at {result}");
    }

    private void HandleAppSelection(DirectoryInfo rootDirectory, List<string> apps)
    {
        string action = ConsoleMenuFactory.PromptForSelection(
            "Choose an action:",
            new List<string> { "Install an application", "Pack a new application", "Uninstall an application", "Exit" });

        if (action == "Install an application")
        {
            PerformInstallation(rootDirectory, apps);
        }
        else if (action == "Pack a new application")
        {
            HandlePackageCreation(rootDirectory);
        }
        else if (action == "Uninstall an application")
        {
            PerformUninstallation();
        }
        else if (action == "Exit")
        {
            Environment.Exit(0);
        }
    }

    private void PerformUninstallation()
    {
        var startMenuDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)));
        var startMenuProgramsDirectory = new DirectoryInfo(Path.Combine(startMenuDirectory.FullName, "Programs"));
        
        var companies = startMenuProgramsDirectory.GetDirectories().Select(d => d.Name).ToList();
        
    }

    private void PerformInstallation(DirectoryInfo rootDirectory, List<string> apps)
    {
        string selectedApp = ConsoleMenuFactory.PromptForSelection("Select an application:", apps);
        List<Version> versions = DirectoryTool.GetVersionsForApp(rootDirectory, selectedApp);
        string selectedVersion = ConsoleMenuFactory.PromptForSelection($"Select a version for {selectedApp}:", versions.Select(v => v.ToString()).ToList());

        bool confirmInstall = ConsoleMenuFactory.PromptForConfirmation($"Install {selectedApp} version {selectedVersion}?");

        if (confirmInstall)
        {
            _installationService.Install(selectedApp, selectedVersion);
        }
    }
}