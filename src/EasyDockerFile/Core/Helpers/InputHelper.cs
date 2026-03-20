using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using static Global.Constants;

namespace EasyDockerFile.Core.Helpers;

public static class InputHelper 
{
    public const string ExitChoice = "Exit";
    public static string AskForInput(string message, string[] options, int? pageSize = null)
    {
        var style = new Style(decoration: Decoration.Bold);
        var prompt = new SelectionPrompt<string>() { SearchEnabled = true }
        .HighlightStyle(style)
        .Title(message)
        .AddChoices(
            options.Select(
                opt => opt.EscapeMarkup()
            )
        )
        .PageSize(
            pageSize ?? Math.Max(options.Length, 3)
        );
        return AnsiConsole.Prompt(prompt);
    }

    public static string AskForInput(string message, IEnumerable<string> options, int? pageSize = null)
    {
        if (!options.Any()) {
            AnsiConsole.Write($"[yellow]{WarningTag}[/] Unable to create selection menu");
            AnsiConsole.Write($"[red]{ErrorTag}[/] Parameter 'options' is empty in InputHelper.AskForInput()");
            Environment.Exit(1);
        }
        
        var style = new Style(decoration: Decoration.Bold);
        var prompt = new SelectionPrompt<string>() { SearchEnabled = true }
        .HighlightStyle(style)
        .Title($"{NLC}{message}")
        .AddChoices(
            options.Select(
                opt => opt.EscapeMarkup()
            )
        )
        .PageSize(
            pageSize ?? Math.Max(options.Count(), 3)
        );
        return AnsiConsole.Prompt(prompt);
    }

    public static string AskForInput(string message)
    {
        var style = new Style(decoration: Decoration.Bold);
        var prompt = new SelectionPrompt<string>() { SearchEnabled = true }
        .HighlightStyle(style)
        .Title(message);
        return AnsiConsole.Prompt(prompt);
    }

    public static IEnumerable<string> MakeInputMenu(IEnumerable<string> options) {
        return options.Append(ExitChoice); 
    }
    public static bool IsExitOption(this string choice) => choice.Equals(ExitChoice);

    public static void UserExitStatusCheck(string inputString) 
    {
        if (inputString.IsExitOption()) {
            Console.WriteLine("[INFO]: Operation cancelled by user.");
            Console.WriteLine("[INFO]: Exiting.");
            Environment.Exit(1);
        }
    }

    // The [NotNull] attribute tells roslyn, if this method returns:
    // The argument 'input' is guaranteed not to be null.
    public static void CheckForNullInput([NotNull] object? input)
    {
        if (input is null) {
            throw new ArgumentNullException(nameof(input), "Selection cannot be null.");
        }
    }

}
