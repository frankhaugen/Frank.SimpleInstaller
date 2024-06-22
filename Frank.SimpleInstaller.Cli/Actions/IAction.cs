namespace Frank.SimpleInstaller.Cli.Actions;

public interface IAction
{
    Task RunAsync();
    
    string Name { get; }
    
    int Order { get; }
}