using Spectre.Console;

namespace Frank.SimpleInstaller.Helpers;

public static class ConsoleMenuHelper
{
    public static string PromptForSelection(string promptTitle, List<string> options)
    {
        SelectionPrompt<string> prompt = new SelectionPrompt<string>()
            .Title(promptTitle)
            .PageSize(10)
            .AddChoices(options);

        return AnsiConsole.Prompt(prompt);
    }

    public static bool PromptForConfirmation(string message)
    {
        return AnsiConsole.Confirm(message);
    }

    public static DirectoryInfo PromptForDirectoryInput(string enterThePathToTheDirectoryToCreateAPackage)
    {
        TextPrompt<string> prompt = new TextPrompt<string>(enterThePathToTheDirectoryToCreateAPackage)
            .PromptStyle("blue")
            .ValidationErrorMessage("Please enter a valid path")
            .Validate(Directory.Exists);

        string path = AnsiConsole.Prompt(prompt);
        return new DirectoryInfo(path);
    }

    public static Version PromptForVersionInput(string enterTheVersionNumberForThePackage)
    {   
        TextPrompt<string> prompt = new TextPrompt<string>(enterTheVersionNumberForThePackage)
            .PromptStyle("blue")
            .ValidationErrorMessage("Please enter a valid version number")
            .Validate(v => Version.TryParse(v, out _));

        string version = AnsiConsole.Prompt(prompt);
        return Version.Parse(version);
    }

    public static string PromptForStringInput(string enterTheNameOfTheApplicationToPackage)
    {
        TextPrompt<string> prompt = new TextPrompt<string>(enterTheNameOfTheApplicationToPackage)
            .PromptStyle("blue")
            .ValidationErrorMessage("Please enter a valid application name")
            .Validate(v => !string.IsNullOrWhiteSpace(v));

        return AnsiConsole.Prompt(prompt);
    }

    public static string? PromptForStringOrNullInput(string enterTheNameOfTheApplicationToPackage)
    {
        TextPrompt<string> prompt = new TextPrompt<string>(enterTheNameOfTheApplicationToPackage)
            .PromptStyle("blue")
            .AllowEmpty();
        var result = AnsiConsole.Prompt(prompt);
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    public static string PromptForFilenameInput(string s)
    {
        TextPrompt<string> prompt = new TextPrompt<string>(s)
            .PromptStyle("blue")
            .ValidationErrorMessage("Please enter a valid filename. The filename cannot contain any of the following characters: " + string.Join(", ", Path.GetInvalidFileNameChars()))
            .Validate(v => !string.IsNullOrWhiteSpace(v) && !v.Any(c => Path.GetInvalidFileNameChars().Contains(c)));

        return AnsiConsole.Prompt(prompt);
    }
}