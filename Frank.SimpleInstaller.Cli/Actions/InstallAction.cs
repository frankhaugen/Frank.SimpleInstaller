namespace Frank.SimpleInstaller.Cli.Actions;

public class InstallAction : IAction
{
    public string Name => "Install an application";
    public int Order => 1;

    public async Task RunAsync()
    {
        // _simpleInstallerService.Install();
    }

    /// <summary>Gives the name of the action.</summary>
    public override string ToString() => Name;
}