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
        [CommandOption("-t|--target <TARGET>")]
        [Description("Target application to uninstall")]
        public string Target { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // Implement your uninstall logic here
        AnsiConsole.MarkupLine("[green]Uninstalling[/] from {0}", settings.Target);
        
        var installationDirectory = OperatingSystemHelper.GetInstallationDirectory(settings.Target);
        var metadataFile = new FileInfo(Path.Combine(installationDirectory.FullName, Constants.MetadataFilename));
        var metadata = InstallationMetadata.Load(metadataFile.OpenRead());
        InstallationHelper.Uninstall(metadata!);
        
        return 0;
    }
}