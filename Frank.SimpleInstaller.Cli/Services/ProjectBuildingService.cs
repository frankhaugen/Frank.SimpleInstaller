using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Logging;

namespace Frank.SimpleInstaller.Cli.Services;

public class ProjectBuildingService
{
    private readonly ILogger<ProjectBuildingService> _logger;

    public ProjectBuildingService(ILogger<ProjectBuildingService> logger)
    {
        _logger = logger;
    }

    public async Task BuildAndPublishAsync(FileInfo projectFile, DirectoryInfo outputDirectory)
    {
        EnsureMsBuildIsRegistered();
        try
        {
            await Task.Run(() => BuildProject(projectFile, outputDirectory));
            _logger.LogInformation("Project built successfully. Output located at: {OutputDir}", outputDirectory.FullName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while building the project.");
            throw; // Re-throw the exception to bubble it up if necessary
        }
    }

    private void EnsureMsBuildIsRegistered()
    {
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
            _logger.LogInformation("MSBuild successfully registered.");
        }
    }

    private void BuildProject(FileInfo projectFile, DirectoryInfo outputDirectory)
    {
        var globalProperties = new Dictionary<string, string>
        {
            { "TargetFramework", "net8.0" },
            { "Configuration", "Release" },
            { "PublishDir", outputDirectory.FullName },
            { "SelfContained", "true" },
            { "PublishSingleFile", "true" },
            { "RuntimeIdentifier", "win-x64" },
            { "PublishTrimmed", "true" }
        };

        var projectCollection = new ProjectCollection();
        var logger = new Microsoft.Build.Logging.ConsoleLogger();
        projectCollection.RegisterLogger(logger);
        var project = new Project(projectFile.FullName, globalProperties, null, projectCollection);

        _logger.LogInformation("Publishing project {ProjectFile} to {OutputDir}...", projectFile.FullName, outputDirectory.FullName);

        if (!project.Build("Restore"))
        {
            throw new InvalidOperationException("Restore failed.");
        }

        if (!project.Build("Build"))
        {
            throw new InvalidOperationException("Build failed.");
        }

        if (!project.Build("Publish"))
        {
            throw new InvalidOperationException("Publish failed.");
        }

        _logger.LogInformation("Project published successfully.");
    }
}