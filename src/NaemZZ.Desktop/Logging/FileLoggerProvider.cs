using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;

namespace NaemZZ.Desktop.Logging;

/// <summary>
/// Minimal, dependency-free file logger provider: appends structured log lines to a daily
/// rolling text file under %LOCALAPPDATA%\NaemZZ\logs. Kept intentionally simple (no external
/// logging package) per "avoid unnecessary architectural complexity", while still satisfying
/// "keep detailed diagnostic information in local log files".
/// </summary>
[ProviderAlias("File")]
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logDirectory;
    private readonly Lock _writeLock = new();
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();

    public FileLoggerProvider(string logDirectory)
    {
        _logDirectory = logDirectory;
        Directory.CreateDirectory(_logDirectory);
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new FileLogger(name, this));

    internal void WriteLine(string line)
    {
        var path = Path.Combine(_logDirectory, $"log-{DateTime.Now:yyyyMMdd}.txt");
        lock (_writeLock)
        {
            File.AppendAllText(path, line + Environment.NewLine);
        }
    }

    public void Dispose() => _loggers.Clear();

    private sealed class FileLogger(string categoryName, FileLoggerProvider provider) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {categoryName}: {message}";
            if (exception is not null)
            {
                line += Environment.NewLine + exception;
            }

            provider.WriteLine(line);
        }
    }
}
