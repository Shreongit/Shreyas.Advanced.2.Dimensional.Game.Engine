namespace SA2DGE.Engine.Graphics;

public abstract class Renderer : IDisposable
{
    private bool _initialized;
    private bool _disposed;

    public bool IsInitialized => _initialized;

    public abstract string Name { get; }

    public void Initialize()
    {
        ThrowIfDisposed();

        if (_initialized)
        {
            return;
        }

        OnInitialize();

        _initialized = true;
    }

    public void BeginFrame()
    {
        ThrowIfDisposed();

        EnsureInitialized();

        OnBeginFrame();
    }

    public void EndFrame()
    {
        ThrowIfDisposed();

        EnsureInitialized();

        OnEndFrame();
    }

    public void Present()
    {
        ThrowIfDisposed();

        EnsureInitialized();

        OnPresent();
    }

    protected abstract void OnInitialize();

    protected abstract void OnBeginFrame();

    protected abstract void OnEndFrame();

    protected abstract void OnPresent();

    public virtual void Shutdown()
    {
        if (!_initialized)
        {
            return;
        }

        OnShutdown();

        _initialized = false;
    }

    protected virtual void OnShutdown()
    {
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Shutdown();

        _disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    protected void EnsureInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException(
                $"Renderer '{Name}' has not been initialized.");
        }
    }
}