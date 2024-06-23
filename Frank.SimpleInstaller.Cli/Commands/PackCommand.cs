using System.ComponentModel;

using Frank.SimpleInstaller.Cli.Helpers;
using Frank.SimpleInstaller.Helpers;
using Frank.SimpleInstaller.Models;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.Commands;

public class PackCommand : AsyncCommand<PackCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-s|--source <SOURCE>")]
        [Description("Source directory to pack")]
        public DirectoryInfo? Source { get; set; }

        [CommandOption("-o|--output <OUTPUT>")]
        [Description("Output directory path")]
        public DirectoryInfo? Output { get; set; }

        [CommandOption("-e|--executable <EXECUTABLE>")]
        [Description("Executable filename")]
        public string? ExecutableName { get; set; }
        
        [CommandOption("-v|--version <VERSION>")]
        [Description("Version number")]
        public Version? Version { get; set; }
        
        [CommandOption("-n|--name <NAME>")]
        [Description("Application name")]
        public string? Name { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new FigletText("App Packer").Centered());
        
        if (settings.Name == null)
        {
            AnsiConsole.MarkupLine("[yellow]Application name not provided[/]");
            string appName = ConsoleMenuHelper.PromptForStringInput("Enter the name of the application to package:");
            settings.Name = appName;
        }
        
        if (settings.Version == null)
        {
            AnsiConsole.MarkupLine("[yellow]Version number not provided[/]");
            Version version = ConsoleMenuHelper.PromptForVersionInput("Enter the version number for the package:");
            settings.Version = version;
        }
        
        if (settings.Source == null)
        {
            AnsiConsole.MarkupLine("[yellow]Source directory not provided[/]");
            var browser = new FileExplorer();
            var source = await browser.GetFolderPathAsync();
            settings.Source = source;
        }
        
        if (settings.ExecutableName == null)
        {
            AnsiConsole.MarkupLine("[yellow]Executable name not provided[/]");
            var browser = new FileExplorer();
            var executable = await browser.GetFilePathAsync(settings.Source);
            settings.ExecutableName = executable.Name;
        }
        
        if (settings.Output == null)
        {
            AnsiConsole.MarkupLine("[yellow]Destination directory not provided[/]");
            var browser = new FileExplorer();
            var output = await browser.GetFolderPathAsync();
            settings.Output = output;
        }
        
        InstallationMetadata metadata = new InstallationMetadata { Name = settings.Name, Version = settings.Version, ExecutableName = settings.ExecutableName};
        
        var appsDirectory = settings.Output;
        
        if (!appsDirectory.Exists)
            appsDirectory.Create();
        
        var sourceDirectory = settings.Source;

        FileInfo result = PackingHelper.Pack(sourceDirectory, appsDirectory, metadata);
        
        AnsiConsole.MarkupLine("[green]Packing successful[/]");
        AnsiConsole.MarkupLine("[blue]{0}[/]", result.FullName);
        
        AnsiConsole.Ask("Press [green]Enter[/] to exit...", "Enter");
        
        return 0;
    }
}