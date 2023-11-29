# 📝 MLogger 📝

**MLogger** - a library for logging your programs. With this library, you can specify your colors, select logging levels, etc.

## How to use ❓
```csharp
var logger = new MLogger();
logger.LogDebug("Hi!");
logger.LogInformation($"Logs here: {logger.GetLogFilePath()}");
```