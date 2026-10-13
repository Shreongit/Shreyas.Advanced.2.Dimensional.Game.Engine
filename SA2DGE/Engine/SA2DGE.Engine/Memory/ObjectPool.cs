namespace SA2DGE.Engine.Memory;

public sealed class ObjectPool<T>
    where T : class
{
    private readonly Stack<T> _available = new();
    private readonly Func<T> _factory;

    private bool _disposed;

    public int Count =>
        _available.Count;

    public int Capacity =>
        _available.Count;

    public ObjectPool(
        Func<T> factory,
        int initialCapacity = 0)
    {
        ArgumentNullException.ThrowIfNull(factory);

        if (initialCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initialCapacity));
        }

        _factory = factory;

        for (int i = 0;
             i < initialCapacity;
             i++)
        {
            _available.Push(
                CreateObject());
        }
    }

    public T Rent()
    {
        ThrowIfDisposed();

        return _available.Count > 0
            ? _available.Pop()
            : CreateObject();
    }

    public void Return(
        T item)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(item);

        _available.Push(item);
    }

    public void Prewarm(
        int count)
    {
        ThrowIfDisposed();

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count));
        }

        for (int i = 0;
             i < count;
             i++)
        {
            _available.Push(
                CreateObject());
        }
    }

    public void Clear()
    {
        ThrowIfDisposed();

        _available.Clear();
    }

    public void Trim(
        int targetCount)
    {
        ThrowIfDisposed();

        if (targetCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetCount));
        }

        while (_available.Count > targetCount)
        {
            _available.Pop();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _available.Clear();
        _disposed = true;
    }

    private T CreateObject()
    {
        T item = _factory();

        ArgumentNullException.ThrowIfNull(item);

        return item;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}