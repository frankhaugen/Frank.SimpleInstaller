using System.ComponentModel;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.Commands;

public class InstallCommand : AsyncCommand<InstallCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-p|--package <PACKAGE>")]
        [Description("Package file to install")]
        public string Package { get; set; }

        [CommandOption("-d|--destination <DESTINATION>")]
        [Description("Destination directory")]
        public string Destination { get; set; }
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // Implement your install logic here
        AnsiConsole.MarkupLine("[green]Installing[/] {0} to {1}", settings.Package, settings.Destination);
        return 0;
    }

}