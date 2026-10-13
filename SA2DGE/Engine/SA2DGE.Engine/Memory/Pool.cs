namespace SA2DGE.Engine.Memory;

public sealed class Pool<T> : IDisposable
    where T : class
{
    private readonly List<T> _items = new();
    private readonly Func<T> _factory;

    private bool _disposed;

    public int Count =>
        _items.Count;

    public int Capacity =>
        _items.Capacity;

    public IReadOnlyList<T> Items =>
        _items;

    public Pool(
        Func<T> factory,
        int capacity = 0)
    {
        ArgumentNullException.ThrowIfNull(factory);

        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity));
        }

        _factory = factory;

        if (capacity > 0)
        {
            _items.Capacity = capacity;
        }
    }

    public T Add()
    {
        ThrowIfDisposed();

        T item = CreateObject();

        _items.Add(item);

        return item;
    }

    public void Add(
        T item)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }

    public T AddAndInitialize(
        Action<T> initializer)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(initializer);

        T item = CreateObject();

        try
        {
            initializer(item);
            _items.Add(item);
            return item;
        }
        catch
        {
            throw;
        }
    }

    public bool Remove(
        T item)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(item);

        return _items.Remove(item);
    }

    public void RemoveAt(
        int index)
    {
        ThrowIfDisposed();

        if ((uint)index >=
            (uint)_items.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index));
        }

        _items.RemoveAt(index);
    }

    public void Clear()
    {
        ThrowIfDisposed();

        _items.Clear();
    }

    public void Reserve(
        int capacity)
    {
        ThrowIfDisposed();

        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity));
        }

        if (_items.Capacity < capacity)
        {
            _items.Capacity = capacity;
        }
    }

    public T this[int index]
    {
        get
        {
            ThrowIfDisposed();

            return _items[index];
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();

        return _items.GetEnumerator();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _items.Clear();
        _items.TrimExcess();

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