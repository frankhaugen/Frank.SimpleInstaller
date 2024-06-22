using System.Reflection;

using Frank.SimpleInstaller.Cli.Actions;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.Commands;

public class MenuCommand : AsyncCommand
{
    private readonly IEnumerable<IAction> _actions;

    public MenuCommand(IEnumerable<IAction> actions)
    {
        _actions = actions;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        AnsiConsole.Write(
            new FigletText($"Simple Installer\nv{Assembly.GetExecutingAssembly().GetName().Version?.ToString(2)}")
                .Centered()
                .Color(Color.Purple));
        
        SelectionPrompt<string> consolePrompt = new SelectionPrompt<string>().PageSize(10).MoreChoicesText("[grey](Move up and down to reveal more actions)[/]").AddChoices(_actions.Select(a => a.Name).ToList());
        
        var selection = Markup.Remove(AnsiConsole.Prompt(consolePrompt));
        
        var selectedAction = _actions.FirstOrDefault(a => a.Name == selection);
        
        if (selectedAction is not null)
        {
            await selectedAction.RunAsync();
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Invalid selection[/]");
            
            await Task.Delay(1000); // wait a bit for the user to read the message
        }
        
        return 0;
    }
}