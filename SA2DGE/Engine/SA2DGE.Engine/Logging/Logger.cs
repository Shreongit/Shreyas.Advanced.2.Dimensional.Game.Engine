namespace SA2DGE.Engine.Logging;

public sealed class Logger : IDisposable
{
    private readonly List<ILogSink> _sinks = new();
    private readonly object _sync = new();

    private bool _disposed;

    public LogLevel MinimumLevel { get; set; }

    public IReadOnlyList<ILogSink> Sinks =>
        _sinks;

    public Logger(
        LogLevel minimumLevel = LogLevel.Trace)
    {
        MinimumLevel = minimumLevel;
    }

    public void AddSink(
        ILogSink sink)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(sink);

        lock (_sync)
        {
            if (!_sinks.Contains(sink))
            {
                _sinks.Add(sink);
            }
        }
    }

    public bool RemoveSink(
        ILogSink sink)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(sink);

        lock (_sync)
        {
            return _sinks.Remove(sink);
        }
    }

    public void Log(
        LogLevel level,
        string message,
        string? category = null,
        Exception? exception = null)
    {
        ThrowIfDisposed();

        if (level < MinimumLevel ||
            level == LogLevel.None)
        {
            return;
        }

        LogMessage logMessage =
            new(
                level,
                message,
                category,
                exception);

        ILogSink[] sinks;

        lock (_sync)
        {
            sinks = _sinks.ToArray();
        }

        foreach (ILogSink sink in sinks)
        {
            sink.Write(logMessage);
        }
    }

    public void Trace(
        string message,
        string? category = null) =>
        Log(
            LogLevel.Trace,
            message,
            category);

    public void Debug(
        string message,
        string? category = null) =>
        Log(
            LogLevel.Debug,
            message,
            category);

    public void Info(
        string message,
        string? category = null) =>
        Log(
            LogLevel.Info,
            message,
            category);

    public void Warning(
        string message,
        string? category = null,
        Exception? exception = null) =>
        Log(
            LogLevel.Warning,
            message,
            category,
            exception);

    public void Error(
        string message,
        string? category = null,
        Exception? exception = null) =>
        Log(
            LogLevel.Error,
            message,
            category,
            exception);

    public void Critical(
        string message,
        string? category = null,
        Exception? exception = null) =>
        Log(
            LogLevel.Critical,
            message,
            category,
            exception);

    public void ClearSinks()
    {
        ThrowIfDisposed();

        lock (_sync)
        {
            _sinks.Clear();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        ILogSink[] sinks;

        lock (_sync)
        {
            sinks = _sinks.ToArray();
            _sinks.Clear();
        }

        foreach (ILogSink sink in sinks)
        {
            sink.Dispose();
        }

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}

public interface ILogSink : IDisposable
{
    void Write(LogMessage message);
}

public sealed class ConsoleLogSink : ILogSink
{
    public void Write(
        LogMessage message)
    {
        Console.WriteLine(
            message.ToString());
    }

    public void Dispose()
    {
    }
}

public sealed class FileLogSink : ILogSink
{
    private readonly StreamWriter _writer;

    private bool _disposed;

    public FileLogSink(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string? directory =
            Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _writer = new StreamWriter(
            path,
            append: true)
        {
            AutoFlush = true
        };
    }

    public void Write(
        LogMessage message)
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);

        _writer.WriteLine(
            message.ToString());
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Dispose();
        _disposed = true;
    }
}