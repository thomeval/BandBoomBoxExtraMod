namespace MediumHardMod;

public static class Output
{
    public static void WriteLine(bool error, string message)
    {
        if (error)
            WriteError(message);
        else
            WriteMessage(message);
    }

    private static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteMessage(message);
        Console.ResetColor();
    }

    private static void WriteMessage(string message) => Console.WriteLine(message);
}