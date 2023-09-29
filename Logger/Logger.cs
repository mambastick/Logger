using System.Runtime.CompilerServices;

namespace LoggerService;

public enum LogLevel
{
    Debug,
    Info,
    Process,
    Success,
    Warning,
    Error,
    Fatal
}

public class Logger
{
    public string logFilePath;

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
    
        var messageLines = message.Split('\n');
    
        foreach (var line in messageLines)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            var logEntry = $"[{timestamp}] [{level.ToString().ToUpper()}] {callerInfo} {line}";

            Console.ForegroundColor = GetConsoleColor(level);
            Console.WriteLine(logEntry);
            Console.ResetColor();

            LogToFile(logEntry);
        }
    }

    private void LogToFile(string logEntry)
    {
        using var writer = File.AppendText(logFilePath);
        writer.WriteLine(logEntry);
    }

    private static ConsoleColor GetConsoleColor(LogLevel level) => level switch
    {
        LogLevel.Debug => ConsoleColor.Gray,
        LogLevel.Info => ConsoleColor.Cyan,
        LogLevel.Process => ConsoleColor.DarkYellow,
        LogLevel.Success => ConsoleColor.Green,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Fatal => ConsoleColor.DarkRed,
        _ => ConsoleColor.Gray
    };
    
    public void LogDebug(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Debug, message, caller);
    public void LogInformation(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Info, message, caller);
    public void LogProcess(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Process, message, caller);
    public void LogSuccess(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Success, message, caller);
    public void LogWarning(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Warning, message, caller);
    public void LogError(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Error, message, caller);
    public void LogFatal(string message, [CallerMemberName] string caller = null) => Log(LogLevel.Fatal, message, caller);
}
