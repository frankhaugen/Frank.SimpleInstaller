using System.ComponentModel;

using Frank.SimpleInstaller.Helpers;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.Commands;

public class InstallCommand : AsyncCommand<InstallCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-p|--package <PACKAGE>")]
        [Description("Package file to install")]
        public string? Package { get; set; }
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new FigletText("App Installer").Centered());
        
        if (settings.Package == null)
        {
            AnsiConsole.MarkupLine("[orange]Source package not provided[/]");
            var browser = new FileExplorer();
            var source = await browser.GetFilePathAsync();
            settings.Package = source.FullName;
        }

        // Validate that package exists and is a valid package zip file
        if (!File.Exists(settings.Package))
        {
            AnsiConsole.MarkupLine("[red]Package file not found[/]");
            await Task.Delay(1000); // Delay for 1 second
            return 1;
        }
        
        if (!settings.Package.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine("[red]Package file is not a valid zip file[/]");
            await Task.Delay(1000); // Delay for 1 second
            return 1;
        }
        
        AnsiConsole.MarkupLine("[green]Installing[/] from {0}", settings.Package);
        
        var succees = InstallationHelper.Install(new FileInfo(settings.Package));
        
        if (succees)
        {
            AnsiConsole.MarkupLine("[green]Installation successful[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Installation failed[/]");
        }
        
        await Task.Delay(1000); // Delay for 1 second
        
        return 0;
    }

}