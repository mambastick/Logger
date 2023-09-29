using System.Runtime.CompilerServices;

namespace LoggerService;

public enum Level
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

    private void Log(Level level, string message, [CallerMemberName] string caller = null)
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

    private static ConsoleColor GetConsoleColor(Level level) => level switch
    {
        Level.Debug => ConsoleColor.Gray,
        Level.Info => ConsoleColor.Cyan,
        Level.Process => ConsoleColor.DarkYellow,
        Level.Success => ConsoleColor.Green,
        Level.Warning => ConsoleColor.Yellow,
        Level.Error => ConsoleColor.Red,
        Level.Fatal => ConsoleColor.DarkRed,
        _ => ConsoleColor.Gray
    };
    
    public void LogDebug(string message, [CallerMemberName] string caller = null) => Log(Level.Debug, message, caller);
    public void LogInformation(string message, [CallerMemberName] string caller = null) => Log(Level.Info, message, caller);
    public void LogProcess(string message, [CallerMemberName] string caller = null) => Log(Level.Process, message, caller);
    public void LogSuccess(string message, [CallerMemberName] string caller = null) => Log(Level.Success, message, caller);
    public void LogWarning(string message, [CallerMemberName] string caller = null) => Log(Level.Warning, message, caller);
    public void LogError(string message, [CallerMemberName] string caller = null) => Log(Level.Error, message, caller);
    public void LogFatal(string message, [CallerMemberName] string caller = null) => Log(Level.Fatal, message, caller);
}
