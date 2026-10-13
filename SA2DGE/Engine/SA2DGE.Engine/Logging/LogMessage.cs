namespace SA2DGE.Engine.Logging;

public readonly struct LogMessage
{
    public DateTime Timestamp { get; }

    public LogLevel Level { get; }

    public string Message { get; }

    public string? Category { get; }

    public Exception? Exception { get; }

    public LogMessage(
        LogLevel level,
        string message,
        string? category = null,
        Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        if (level == LogLevel.None)
        {
            throw new ArgumentException(
                "Log message level cannot be None.",
                nameof(level));
        }

        Timestamp = DateTime.UtcNow;
        Level = level;
        Message = message;
        Category = category;
        Exception = exception;
    }

    public override string ToString()
    {
        string category =
            string.IsNullOrWhiteSpace(Category)
                ? string.Empty
                : $"[{Category}] ";

        string exception =
            Exception is null
                ? string.Empty
                : $" {Exception}";

        return
            $"{Timestamp:O} " +
            $"[{Level}] " +
            category +
            Message +
            exception;
    }
}