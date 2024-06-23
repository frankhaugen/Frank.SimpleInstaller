using Frank.SimpleInstaller.Cli.DependencyInjection;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.Commands;

public class DefaultCommand : AsyncCommand
{
    private readonly PackCommand _packCommand;
    private readonly InstallCommand _installCommand;
    private readonly UninstallCommand _uninstallCommand;
    
    public DefaultCommand(PackCommand packCommand, InstallCommand installCommand, UninstallCommand uninstallCommand)
    {
        _packCommand = packCommand;
        _installCommand = installCommand;
        _uninstallCommand = uninstallCommand;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        AnsiConsole.Write(new FigletText("Simple Installer").Centered());
        AnsiConsole.MarkupLine("Welcome to the Simple Installer CLI, please select a command to run.");
        
        var options = new List<(string Name, string Description)>
        {
            ("pack", "Package an application"),
            ("install", "Install a package"),
            ("uninstall", "Uninstall an application")
        };
        
        var selection = AnsiConsole.Prompt(new SelectionPrompt<(string Name, string Description)>()
            .Title("Select a command")
            .PageSize(10)
            .AddChoices(options)
            .UseConverter(tuple => tuple.Name + " - " + tuple.Description)
        );
        
        AnsiConsole.Clear();

        return selection.Name switch
        {
            "pack" => await _packCommand.ExecuteAsync(context, new PackCommand.Settings()),
            "install" => await _installCommand.ExecuteAsync(context, new InstallCommand.Settings()),
            "uninstall" => await _uninstallCommand.ExecuteAsync(context, new UninstallCommand.Settings()),
            _ => 0
        };
    }
}

