using System.Diagnostics;
using System.Drawing;
using Serilog;
using Serilog.Events;

namespace LoggerService;

public static class LoggerFactory
{
    private static readonly ILogger _logger;

    static LoggerFactory()
    {
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logPath);

        const string logInfo =
            "[{Timestamp:dd-MM-yyyy HH:mm:ss}] [{Level:u3}] {SourceContext} ({Caller}) {Message:lj}{NewLine}{Exception}";
        _logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(outputTemplate: logInfo, restrictedToMinimumLevel: LogEventLevel.Verbose)
            .WriteTo.File(Path.Combine(logPath, $"{DateTime.Now:dd-MM-yyyy_HH:mm:ss}.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: logInfo,
                restrictedToMinimumLevel: LogEventLevel.Verbose)
            .CreateLogger();
    }

    public static ILogger GetLogger() => _logger;

    public static string GetCurrentMethodName()
    {
        var method = new StackFrame(1).GetMethod();
        return $"{method.DeclaringType?.FullName}.{method.Name}";
    }
    
    private static ConsoleColor GetConsoleColor(LogEventLevel level) => level switch
    {
        LogEventLevel.Debug => ConsoleColor.Blue,
        LogEventLevel.Information => ConsoleColor.Cyan,
        LogEventLevel.Warning => ConsoleColor.Yellow,
        LogEventLevel.Error => ConsoleColor.Red,
        LogEventLevel.Fatal => ConsoleColor.DarkRed,
        _ => ConsoleColor.White
    };
}