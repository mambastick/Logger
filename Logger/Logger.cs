using System.Runtime.CompilerServices;

namespace LoggerService;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}

public class Logger
{
    private string logFilePath;

    public Logger()
    {
        var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logFolder);

        var logFileName = $"{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.log";
        logFilePath = Path.Combine(logFolder, logFileName);
    }

    private void Log(LogLevel level, string message, [CallerMemberName] string caller = null)
    {
        var timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        var callerInfo = string.IsNullOrWhiteSpace(caller) ? "" : $"[{caller}]";
        var logEntry = $"[{timestamp}] [{level}] {callerInfo} {message}";

        Console.ForegroundColor = GetConsoleColor(level);
        Console.WriteLine(logEntry);
        Console.ResetColor();

        LogToFile(logEntry);
    }

    private void LogToFile(string logEntry)
    {
        using var writer = File.AppendText(logFilePath);
        writer.WriteLine(logEntry);
    }

    private static ConsoleColor GetConsoleColor(LogLevel level) => level switch
    {
        LogLevel.Debug => ConsoleColor.White,
        LogLevel.Info => ConsoleColor.Cyan,
        LogLevel.Warning => ConsoleColor.DarkYellow,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Fatal => ConsoleColor.DarkRed,
        _ => ConsoleColor.Gray
    };
    
    public void LogDebug(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Debug, message, caller);
    public void LogInformation(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Info, message, caller);
    public void LogWarning(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Warning, message, caller);
    public void LogError(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Error, message, caller);
    public void LogFatal(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Fatal, message, caller);
}
