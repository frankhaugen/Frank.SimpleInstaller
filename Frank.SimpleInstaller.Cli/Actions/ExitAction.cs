using Spectre.Console;

namespace Frank.SimpleInstaller.Cli.Actions;

public class ExitAction : IAction
{
    public string Name => "Exit";
    public int Order => int.MaxValue;

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("Goodbye!");
        await Task.Delay(1000); // wait a bit for the user to read the message
        Environment.Exit(0);
    }

    /// <summary>Gives the name of the action.</summary>
    public override string ToString() => Name;
}