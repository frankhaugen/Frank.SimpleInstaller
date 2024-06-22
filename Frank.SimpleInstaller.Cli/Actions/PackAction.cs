using Frank.SimpleInstaller.Helpers;

namespace Frank.SimpleInstaller.Cli.Actions;

public class PackAction : IAction
{
    public string Name => "Pack a new application";
    public int Order => 0;

    public async Task RunAsync()
    {
        var rootDirectory = DirectoryHelper.GetBaseDirectory();
        
        
    }

    /// <summary>Gives the name of the action.</summary>
    public override string ToString() => Name;
}