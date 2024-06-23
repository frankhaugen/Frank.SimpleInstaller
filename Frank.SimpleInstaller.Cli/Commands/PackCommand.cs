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
        public string? Source { get; set; }

        [CommandOption("-o|--output <OUTPUT>")]
        [Description("Output file path")]
        public string? Output { get; set; }

        [CommandOption("-e|--executable <EXECUTABLE>")]
        [Description("Executable filename")]
        public string? ExecutableName { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new FigletText("App Packer").Centered());
        
        if (settings.Source == null)
        {
            AnsiConsole.MarkupLine("[yellow]Source directory not provided[/]");
            var browser = new FileExplorer();
            var source = await browser.GetFolderPathAsync();
            settings.Source = source.FullName;
        }
        
        if (settings.ExecutableName == null)
        {
            AnsiConsole.MarkupLine("[yellow]Executable name not provided[/]");
            var browser = new FileExplorer();
            var executable = await browser.GetFilePathAsync(new DirectoryInfo(settings.Source));
            settings.ExecutableName = executable.Name;
        }
        
        if (settings.Output == null)
        {
            AnsiConsole.MarkupLine("[yellow]Destination directory not provided[/]");
            var browser = new FileExplorer();
            var output = await browser.GetFolderPathAsync();
            settings.Output = output.FullName;
        }
        
        AnsiConsole.MarkupLine("[green]Packing[/] from {0} to {1}", settings.Source, settings.Output);
        
        Version version = ConsoleMenuHelper.PromptForVersionInput("Enter the version number for the package:");
        string appName = ConsoleMenuHelper.PromptForStringInput("Enter the name of the application to package:");

        string? safeAppName = null;
        if (appName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            safeAppName = ConsoleMenuHelper.PromptForFilenameInput($"The application name '{appName}' contains invalid characters,({string.Join(", ", Path.GetInvalidFileNameChars())}). Enter a safe name for the package:");
        }
        
        var company = ConsoleMenuHelper.PromptForStringOrNullInput("Enter the name of the company that created the application (optional):");
        
        InstallationMetadata metadata = new InstallationMetadata { Name = appName, Version = version, SafeName = safeAppName, Company = company, ExecutableName = settings.ExecutableName};
        
        var appsDirectory = new DirectoryInfo(settings.Output);
        
        if (!appsDirectory.Exists)
            appsDirectory.Create();
        
        var sourceDirectory = new DirectoryInfo(settings.Source);

        FileInfo result = PackingHelper.Pack(sourceDirectory, appsDirectory, metadata);
        
        AnsiConsole.MarkupLine("[green]Packing successful[/]");
        AnsiConsole.MarkupLine("[blue]{0}[/]", result.FullName);
        
        AnsiConsole.Ask("Press [green]Enter[/] to exit...", "Enter");
        
        return 0;
    }
}