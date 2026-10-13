namespace SA2DGE.Engine.Platform.Window;

public sealed class Window : IDisposable
{
    private readonly IWindowBackend _backend;

    private bool _disposed;

    public string Title => _backend.Title;

    public int Width => _backend.Width;

    public int Height => _backend.Height;

    public bool IsOpen => _backend.IsOpen;

    public bool VSync { get; private set; }

    public bool Resizable { get; private set; }

    public bool Fullscreen { get; private set; }

    public bool Borderless { get; private set; }

    internal nint NativeHandle => _backend.NativeHandle;

    public Window(WindowConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        VSync = config.VSync;
        Resizable = config.Resizable;
        Fullscreen = config.Fullscreen;
        Borderless = config.Borderless;

        _backend = CreateBackend(config);
    }

    public void Open()
    {
        ThrowIfDisposed();

        _backend.Open();
    }

    public void Close()
    {
        if (_disposed)
            return;

        _backend.Close();
    }

    public void ProcessEvents()
    {
        ThrowIfDisposed();

        _backend.ProcessEvents();
    }

    public bool TryGetEvent(out WindowEvent windowEvent)
    {
        ThrowIfDisposed();

        return _backend.TryGetEvent(out windowEvent);
    }

    public void Resize(int width, int height)
    {
        ThrowIfDisposed();

        _backend.Resize(width, height);
    }

    public void SetTitle(string title)
    {
        ThrowIfDisposed();

        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        _backend.SetTitle(title);
    }

    public void SetVSync(bool enabled)
    {
        ThrowIfDisposed();

        VSync = enabled;
        _backend.SetVSync(enabled);
    }

    public void SetResizable(bool enabled)
    {
        ThrowIfDisposed();

        Resizable = enabled;
        _backend.SetResizable(enabled);
    }

    public void SetFullscreen(bool enabled)
    {
        ThrowIfDisposed();

        Fullscreen = enabled;
        _backend.SetFullscreen(enabled);
    }

    public void SetBorderless(bool enabled)
    {
        ThrowIfDisposed();

        Borderless = enabled;
        _backend.SetBorderless(enabled);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _backend.Dispose();

        _disposed = true;
    }

    private static IWindowBackend CreateBackend(WindowConfig config)
    {
        if (OperatingSystem.IsWindows())
            return new WindowsWindowBackend(config);

        throw new PlatformNotSupportedException(
            "No window backend is available for the current operating system.");
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}