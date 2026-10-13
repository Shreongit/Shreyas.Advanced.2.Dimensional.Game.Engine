namespace SA2DGE.Engine.Events;

public abstract class Event
{
    public DateTime Timestamp { get; }

    public bool Handled { get; private set; }

    protected Event()
    {
        Timestamp = DateTime.UtcNow;
    }

    public void MarkHandled()
    {
        Handled = true;
    }

    public void ResetHandled()
    {
        Handled = false;
    }

    public override string ToString()
    {
        return GetType().Name;
    }
}

public sealed class EventHandler
{
    public Type EventType { get; }

    public EventHandler(
        Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

        if (!typeof(Event).IsAssignableFrom(eventType))
        {
            throw new ArgumentException(
                $"Type '{eventType.FullName}' must derive from Event.",
                nameof(eventType));
        }

        EventType = eventType;
    }
}