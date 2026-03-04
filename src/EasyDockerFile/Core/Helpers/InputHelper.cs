using Spectre.Console;
using static EasyDockerFile.Core.Common.Constants;

namespace EasyDockerFile.Core.Helpers;

public static class InputHelper 
{
    public const string ExitChoice = "Exit";
    public static string AskForInput(string message, string[] options)
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
        .PageSize(Math.Max(options.Length, 3));
        return AnsiConsole.Prompt(prompt);
    }

    public static string AskForInput(string message, IEnumerable<string> options)
    {
        var style = new Style(decoration: Decoration.Bold);
        var prompt = new SelectionPrompt<string>() { SearchEnabled = true }
        .HighlightStyle(style)
        .Title($"{NLC}{message}")
        .AddChoices(
            options.Select(
                opt => opt.EscapeMarkup()
            )
        )
        .PageSize(Math.Max(options.Count(), 3));
        return AnsiConsole.Prompt(prompt);
    }

    public static IEnumerable<string> MakeInputMenu(IEnumerable<string> options) {
        return options.Append(ExitChoice); 
    }
    public static bool IsExitOption(this string choice) => choice.Equals(ExitChoice);

}
