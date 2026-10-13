namespace SA2DGE.Engine.Events;

public sealed class EventDispatcher
{
    private readonly Dictionary<Type, List<Subscription>> _subscriptions = new();

    public int SubscriptionCount { get; private set; }

    public void Subscribe<TEvent>(
        Action<TEvent> handler)
        where TEvent : Event
    {
        ArgumentNullException.ThrowIfNull(handler);

        Type eventType = typeof(TEvent);

        if (!_subscriptions.TryGetValue(
                eventType,
                out List<Subscription>? handlers))
        {
            handlers = new List<Subscription>();
            _subscriptions.Add(eventType, handlers);
        }

        handlers.Add(
            new Subscription(
                handler,
                typeof(TEvent)));

        SubscriptionCount++;
    }

    public bool Unsubscribe<TEvent>(
        Action<TEvent> handler)
        where TEvent : Event
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (!_subscriptions.TryGetValue(
                typeof(TEvent),
                out List<Subscription>? handlers))
        {
            return false;
        }

        for (int i = handlers.Count - 1;
             i >= 0;
             i--)
        {
            if (!ReferenceEquals(
                    handlers[i].Handler,
                    handler))
            {
                continue;
            }

            handlers.RemoveAt(i);
            SubscriptionCount--;

            if (handlers.Count == 0)
            {
                _subscriptions.Remove(
                    typeof(TEvent));
            }

            return true;
        }

        return false;
    }

    public bool Dispatch<TEvent>(
        TEvent eventInstance)
        where TEvent : Event
    {
        ArgumentNullException.ThrowIfNull(eventInstance);

        if (!_subscriptions.TryGetValue(
                typeof(TEvent),
                out List<Subscription>? handlers))
        {
            return eventInstance.Handled;
        }

        Subscription[] snapshot =
            handlers.ToArray();

        foreach (Subscription subscription in snapshot)
        {
            if (eventInstance.Handled)
            {
                break;
            }

            subscription.Invoke(
                eventInstance);
        }

        return eventInstance.Handled;
    }

    public bool Dispatch(
        Event eventInstance)
    {
        ArgumentNullException.ThrowIfNull(eventInstance);

        Type eventType =
            eventInstance.GetType();

        if (!_subscriptions.TryGetValue(
                eventType,
                out List<Subscription>? handlers))
        {
            return eventInstance.Handled;
        }

        Subscription[] snapshot =
            handlers.ToArray();

        foreach (Subscription subscription in snapshot)
        {
            if (eventInstance.Handled)
            {
                break;
            }

            subscription.Invoke(
                eventInstance);
        }

        return eventInstance.Handled;
    }

    public void Clear()
    {
        _subscriptions.Clear();
        SubscriptionCount = 0;
    }

    private sealed class Subscription
    {
        public Delegate Handler { get; }

        private Type EventType { get; }

        public Subscription(
            Delegate handler,
            Type eventType)
        {
            Handler = handler;
            EventType = eventType;
        }

        public void Invoke(
            Event eventInstance)
        {
            if (eventInstance.GetType() != EventType)
            {
                return;
            }

            Handler.DynamicInvoke(
                eventInstance);
        }
    }
}