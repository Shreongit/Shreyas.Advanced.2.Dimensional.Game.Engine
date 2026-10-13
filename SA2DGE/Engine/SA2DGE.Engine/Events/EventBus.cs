namespace SA2DGE.Engine.Events;

public sealed class EventBus
{
    private readonly EventDispatcher _dispatcher = new();

    public int SubscriptionCount =>
        _dispatcher.SubscriptionCount;

    public void Subscribe<TEvent>(
        Action<TEvent> handler)
        where TEvent : Event
    {
        _dispatcher.Subscribe(handler);
    }

    public bool Unsubscribe<TEvent>(
        Action<TEvent> handler)
        where TEvent : Event
    {
        return _dispatcher.Unsubscribe(handler);
    }

    public bool Publish<TEvent>(
        TEvent eventInstance)
        where TEvent : Event
    {
        ArgumentNullException.ThrowIfNull(eventInstance);

        return _dispatcher.Dispatch(
            eventInstance);
    }

    public bool Publish(
        Event eventInstance)
    {
        ArgumentNullException.ThrowIfNull(eventInstance);

        return _dispatcher.Dispatch(
            eventInstance);
    }

    public void Clear()
    {
        _dispatcher.Clear();
    }
}