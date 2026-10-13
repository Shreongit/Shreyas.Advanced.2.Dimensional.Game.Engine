using SA2DGE.Engine.ECS;

namespace SA2DGE.Engine.Scene;

public sealed class Scene : IDisposable
{
    private bool _initialized;
    private bool _disposed;

    public string Name { get; }

    public World World { get; }

    public bool IsInitialized => _initialized;

    public Scene(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        World = new World();
    }

    public void Initialize()
    {
        ThrowIfDisposed();

        if (_initialized)
        {
            return;
        }

        _initialized = true;
    }

    public void Update(float deltaTime)
    {
        ThrowIfDisposed();

        if (!_initialized)
        {
            throw new InvalidOperationException(
                $"Scene '{Name}' must be initialized before updating.");
        }
    }

    public void Shutdown()
    {
        ThrowIfDisposed();

        if (!_initialized)
        {
            return;
        }

        _initialized = false;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_initialized)
        {
            Shutdown();
        }

        World.Dispose();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    public override string ToString()
    {
        return Name;
    }
}