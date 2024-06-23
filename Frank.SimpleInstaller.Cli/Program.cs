using Frank.SimpleInstaller.Cli.Commands;
using Frank.SimpleInstaller.Cli.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;
using Spectre.Console.Cli;

AnsiConsole.Profile.Encoding = System.Text.Encoding.UTF8;

var services = new ServiceCollection();

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<PackCommand>("pack");
    config.AddCommand<InstallCommand>("install");
    config.AddCommand<UninstallCommand>("uninstall");
});

await app.RunAsync(args);

AnsiConsole.MarkupLine("Goodbye!");
Console.ReadLine();