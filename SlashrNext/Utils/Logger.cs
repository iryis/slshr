namespace SlashrNext.Utils;

public static class Logger
{
    private static string prefix => "[Slashr] ";

    public static void Msg(string msg, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("] ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(prefix);

        Console.ForegroundColor = color;
        Console.Write($"{msg}\n");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Log(string msg, ConsoleColor color = ConsoleColor.White) => Msg(msg, color);


    public static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("] ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(prefix);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"[WARNING] {msg}\n");
        Console.ResetColor();
    }

    public static void Error(Exception exception) => Error(exception.ToString());

    public static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("] ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(prefix);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"[ERROR] {msg}\n");
        Console.ResetColor();
    }
}