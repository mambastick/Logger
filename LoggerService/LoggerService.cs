using Serilog;
using Serilog.Events;

namespace LoggerService;

public class LoggerService
{
    public static ILogger CreateLogger()
    {
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        Directory.CreateDirectory(logPath);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose() // Уровень логирования по умолчанию
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information) // Цветной вывод в консоль
            .WriteTo.File(
                Path.Combine(logPath, $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-app.log"),
                rollingInterval: RollingInterval.Day, // Ротация логов по дням
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Verbose) // Запись в файл
            .CreateLogger();

        return logger;
    }
}