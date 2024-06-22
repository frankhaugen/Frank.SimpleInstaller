namespace Frank.SimpleInstaller.Cli.Actions;

public class UninstallAction : IAction
{
    public string Name => "Uninstall an application";
    public int Order => 2;

    public async Task RunAsync()
    {
        // _simpleInstallerService.Uninstall();
    }

    /// <summary>Gives the name of the action.</summary>
    public override string ToString() => Name;
}