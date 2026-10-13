namespace SA2DGE.Engine.ECS;

public sealed class SystemGroup : IDisposable
{
    private readonly List<System> _systems = new();

    private World? _world;
    private bool _initialized;
    private bool _disposed;

    public int Count => _systems.Count;

    public IReadOnlyList<System> Systems => _systems;

    public void Add(System system)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(system);

        if (_systems.Contains(system))
        {
            return;
        }

        _systems.Add(system);

        if (_initialized && _world is not null)
        {
            system.Initialize(_world);
        }
    }

    public bool Remove(System system)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(system);

        if (!_systems.Remove(system))
        {
            return false;
        }

        if (_initialized && _world is not null)
        {
            system.Shutdown(_world);
        }

        return true;
    }

    public void Initialize(World world)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(world);

        if (_initialized)
        {
            return;
        }

        _world = world;

        foreach (System system in _systems)
        {
            system.Initialize(world);
        }

        _initialized = true;
    }

    public void Update(float deltaTime)
    {
        ThrowIfDisposed();

        if (!_initialized || _world is null)
        {
            throw new InvalidOperationException(
                "The system group must be initialized before updating.");
        }

        foreach (System system in _systems)
        {
            if (system.Enabled)
            {
                system.Update(
                    _world,
                    deltaTime);
            }
        }
    }

    public void Shutdown()
    {
        ThrowIfDisposed();

        if (!_initialized || _world is null)
        {
            return;
        }

        for (int i = _systems.Count - 1; i >= 0; i--)
        {
            _systems[i].Shutdown(_world);
        }

        _world = null;
        _initialized = false;
    }

    public void Clear()
    {
        ThrowIfDisposed();

        if (_initialized && _world is not null)
        {
            for (int i = _systems.Count - 1; i >= 0; i--)
            {
                _systems[i].Shutdown(_world);
            }

            _world = null;
            _initialized = false;
        }

        _systems.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Shutdown();

        _systems.Clear();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}