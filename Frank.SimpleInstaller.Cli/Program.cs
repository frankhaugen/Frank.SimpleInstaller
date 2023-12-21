using Frank.SimpleInstaller;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<IInstallationService, InstallationService>();
services.AddSingleton<IPackingService, PackingService>();
services.AddSingleton<ISimpleInstallerService, SimpleInstallerService>();

var serviceProvider = services.BuildServiceProvider();
var simpleInstallerService = serviceProvider.GetRequiredService<ISimpleInstallerService>();
var rootDirectory = new DirectoryInfo(AppContext.BaseDirectory);

try
{
    simpleInstallerService.Run(rootDirectory);
}
catch (Exception e)
{
    Console.WriteLine("An error occurred:");
    Console.WriteLine(e);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();