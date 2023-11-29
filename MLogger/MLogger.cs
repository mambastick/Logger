using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace LoggerService;

public enum MLogLevel
{
    Debug,
    Info,
    Process,
    Success,
    Warning,
    Error,
    Fatal
}

public class MLogger
{
    private readonly string LogFilePath;
    private readonly Queue<string> logQueue = new Queue<string>();
    private readonly object logLock = new object();
    private bool isWriting = false;

    public MLogger()
    {
        var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logFolder);

        var logFileName = $"{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.log";
        LogFilePath = Path.Combine(logFolder, logFileName);

        Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
    }

    private void Log(MLogLevel level, string message, [CallerMemberName] string caller = null)
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

    private static ConsoleColor GetConsoleColor(MLogLevel level) => level switch
    {
        MLogLevel.Debug => ConsoleColor.Gray,
        MLogLevel.Info => ConsoleColor.Cyan,
        MLogLevel.Process => ConsoleColor.DarkYellow,
        MLogLevel.Success => ConsoleColor.Green,
        MLogLevel.Warning => ConsoleColor.Yellow,
        MLogLevel.Error => ConsoleColor.Red,
        MLogLevel.Fatal => ConsoleColor.DarkRed,
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
        Log(MLogLevel.Info, message, caller);

    public void LogProcess(string message, [CallerMemberName] string caller = null) =>
        Log(MLogLevel.Process, message, caller);

    public void LogSuccess(string message, [CallerMemberName] string caller = null) =>
        Log(MLogLevel.Success, message, caller);

    public void LogWarning(string message, [CallerMemberName] string caller = null) =>
        Log(MLogLevel.Warning, message, caller);

    public void LogError(string message, [CallerMemberName] string caller = null) => Log(MLogLevel.Error, message, caller);
    public void LogFatal(string message, [CallerMemberName] string caller = null) => Log(MLogLevel.Fatal, message, caller);
}