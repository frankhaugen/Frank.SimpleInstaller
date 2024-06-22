using System.ComponentModel;

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
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new FigletText("App Packer").Centered());
        
        if (settings.Source == null)
        {
            AnsiConsole.MarkupLine("[orange]Source directory not provided[/]");
            var browser = new FileExplorer();
            var source = await browser.GetFolderPathAsync();
            settings.Source = source.FullName;
        }
        
        if (settings.Output == null)
        {
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
        
        InstallationMetadata metadata = new InstallationMetadata { Name = appName, Version = version, SafeName = safeAppName, Company = company };
        
        var appsDirectory = new DirectoryInfo(settings.Output);
        
        if (!appsDirectory.Exists)
            appsDirectory.Create();
        
        var sourceDirectory = new DirectoryInfo(settings.Source);

        FileInfo result = PackingHelper.Pack(sourceDirectory, appsDirectory, metadata);
        
        await Task.Delay(1000); // wait a bit for the user to read the message
        
        return 0;
    }
}