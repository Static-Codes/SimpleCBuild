namespace Global;


// A <PackageReference> to Spectre.Console must be included prior to Referencing Logging.cs in a .csproj
using Spectre.Console;
using System.Diagnostics.CodeAnalysis;
using static Global.Constants;

internal static class Logging
{
    public static void WriteWarningMessage(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{WarningTag}[/] {message.EscapeMarkup()}");
    }

    public static void WriteErrorMessage(string message, int? exitCode = null, [DoesNotReturnIf(true)] bool exit = false)
    {
        AnsiConsole.MarkupLine($"[red]{ErrorTag}[/] {message.EscapeMarkup()}");
        if (exit)
        {
            Environment.Exit(exitCode ?? 1);
        }
    }

    public static void WriteSuccessMessage(string message)
    {
        AnsiConsole.MarkupLine($"[green]{SuccessTag}[/] {message.EscapeMarkup()}");
    }
}
