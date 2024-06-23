using System.ComponentModel;

using Frank.SimpleInstaller.Helpers;
using Frank.SimpleInstaller.Models;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.Commands;

public class UninstallCommand : AsyncCommand<UninstallCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-n|--name <NAME>")]
        [Description("Target application to uninstall")]
        public string? Name { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new FigletText("App Uninstaller").Centered());
        
        if (settings.Name == null)
        {
            AnsiConsole.MarkupLine("[yellow]Target application name not provided[/]");
            
            // Create selection menu for installed applications
            var installedApplications = InstallationHelper.GetInstalledApplications();
            
            var selection = AnsiConsole.Prompt(new SelectionPrompt<KeyValuePair<string, DirectoryInfo>>()
                .Title("Select an application to uninstall")
                .PageSize(10)
                .AddChoices(installedApplications)
                .UseConverter(pair => pair.Key)
            );
            
            settings.Name = selection.Key;
        }
        
        AnsiConsole.MarkupLine("[green]Uninstalling[/] from {0}", settings.Name);
        
        var exitCode = 0;
        
        try
        {
            InstallationMetadata metadata = GetInstallationMetadata(settings);
            var success = InstallationHelper.Uninstall(metadata);

            if (success)
            {
                AnsiConsole.MarkupLine("[green]Uninstallation successful[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Uninstallation failed[/]");
            }
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]Uninstallation failed[/]");
            AnsiConsole.WriteException(e);
            exitCode = 1;
        }
        
        AnsiConsole.MarkupLine("[yellow]Press any key to exit[/]");
        Console.ReadKey();
        
        return exitCode;
    }

    private static InstallationMetadata GetInstallationMetadata(Settings settings)
    {
        var installationDirectory = OperatingSystemHelper.GetInstallationDirectory(settings.Name!);
        var metadataFile = new FileInfo(Path.Combine(installationDirectory.FullName, Constants.MetadataFilename));
        using var fileStream = metadataFile.OpenRead();
        var metadata = InstallationMetadata.Load(fileStream);
        fileStream.Close();
        return metadata ?? throw new Exception("Failed to load installation metadata");
    }
}