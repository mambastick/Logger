using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

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
    private readonly string LogFilePath;
    private readonly Queue<string> logQueue = new Queue<string>();
    private readonly object logLock = new object();
    private bool isWriting = false;

    public Logger()
    {
        var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logFolder);

        var logFileName = $"{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.log";
        LogFilePath = Path.Combine(logFolder, logFileName);

        Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
    }

    private void Log(Level level, string message, [CallerMemberName] string caller = null)
    {
        var timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

        if (caller == ".ctor")
            caller =
                $"{new System.Diagnostics.StackTrace(1).GetFrame(1)?.GetMethod()?.ReflectedType?.Name} constructor";

        var callerInfo = string.IsNullOrWhiteSpace(caller) ? "" : $"[{caller}]";

        var messageLines = message.Split('\n');

        foreach (var line in messageLines)
        {
            var logEntry = $"[{timestamp}] [{level.ToString().ToUpper()}] {callerInfo} {line}";

            Console.ForegroundColor = GetConsoleColor(level);
            Console.WriteLine(logEntry);
            Console.ResetColor();

            lock (logLock)
            {
                logQueue.Enqueue(logEntry);
            }
        }
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
    
    private void ProcessLogQueue()
    {
        while (true)
        {
            string logEntry = null;

            lock (logLock)
                if (logQueue.Count > 0)
                    logEntry = logQueue.Dequeue();

            if (logEntry != null)
                WriteLogToFile(logEntry);
            else
                Thread.Sleep(100);
        }
    }

    private async void WriteLogToFile(string logEntry)
    {
        while (isWriting)
            await Task.Delay(10);

        isWriting = true;
        try
        {
            await using var writer = File.AppendText(LogFilePath);
            await writer.WriteLineAsync(logEntry);
        }
        finally
        {
            isWriting = false;
        }
    }

    public string GetLogFilePath() => LogFilePath;
    public void LogDebug(string message, [CallerMemberName] string caller = null) => Log(Level.Debug, message, caller);

    public void LogInformation(string message, [CallerMemberName] string caller = null) =>
        Log(Level.Info, message, caller);

    public void LogProcess(string message, [CallerMemberName] string caller = null) =>
        Log(Level.Process, message, caller);

    public void LogSuccess(string message, [CallerMemberName] string caller = null) =>
        Log(Level.Success, message, caller);

    public void LogWarning(string message, [CallerMemberName] string caller = null) =>
        Log(Level.Warning, message, caller);

    public void LogError(string message, [CallerMemberName] string caller = null) => Log(Level.Error, message, caller);
    public void LogFatal(string message, [CallerMemberName] string caller = null) => Log(Level.Fatal, message, caller);
}