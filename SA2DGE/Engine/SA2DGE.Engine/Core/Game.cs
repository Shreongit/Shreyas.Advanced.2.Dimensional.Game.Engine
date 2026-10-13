using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Platform.Input;
using SA2DGE.Engine.Platform.Window;


namespace SA2DGE.Engine.Core;

public abstract class Game : IDisposable
{
    private bool _initialized;
    private bool _shutdown;
    private bool _disposed;

    private IInputBackend? _inputBackend;

    public Window Window { get; }

    public Input Input { get; }

    public GraphicsBackend? Graphics { get; private set; }
    
    public GraphicsCommands? GraphicsCommands =>
        Graphics?.Commands;
    
    public GraphicsShaders? GraphicsShaders =>
        Graphics?.Shaders;

    public bool IsInitialized =>
        _initialized;

    public bool IsShutdown =>
        _shutdown;

    protected Game(WindowConfig? windowConfig = null)
    {
        Window = new Window(
            windowConfig ?? new WindowConfig());

        Input = new Input();
    }

    public virtual void Initialize()
    {
    }

    public virtual void Update(
        float deltaTime)
    {
    }

    public virtual void Render()
    {
    }

    public virtual void Shutdown()
    {
    }

    public virtual void OnWindowEvent(WindowEvent windowEvent)
    {
        if (windowEvent.Type == WindowEventType.MouseWheel)
        {
            Input.AddMouseWheelDelta(
                windowEvent.WheelDelta);

            return;
        }

        if (windowEvent.Type == WindowEventType.Resized &&
            windowEvent.Width > 0 &&
            windowEvent.Height > 0)
        {
            if (Graphics is not null &&
                (Graphics.Width != windowEvent.Width ||
                 Graphics.Height != windowEvent.Height))
            {
                Graphics.Resize(
                    windowEvent.Width,
                    windowEvent.Height);
            }
        }
    }

    internal void UpdateInput()
    {
        _inputBackend?.Update(Input);
    }

    internal void InitializeRuntime()
    {
        ThrowIfDisposed();

        if (_initialized)
            return;

        Window.Open();

        try
        {
            _inputBackend = CreateInputBackend();
            _inputBackend.Initialize(Window);

            Graphics =
                new Direct3D11GraphicsBackend();

            Graphics.Initialize(Window);

            Initialize();

            _initialized = true;
            _shutdown = false;
        }
        catch
        {
            Graphics?.Dispose();
            Graphics = null;

            _inputBackend?.Shutdown();
            _inputBackend?.Dispose();
            _inputBackend = null;

            Input.Clear();

            Window.Close();

            throw;
        }
    }

    private static IInputBackend CreateInputBackend()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsInputBackend();
        }

        throw new PlatformNotSupportedException(
            "No input backend is available for the current operating system.");
    }

    internal void ShutdownRuntime()
    {
        if (_shutdown)
            return;

        try
        {
            Shutdown();
        }
        finally
        {
            Graphics?.Dispose();
            Graphics = null;

            _inputBackend?.Shutdown();
            _inputBackend?.Dispose();
            _inputBackend = null;

            Input.Clear();

            Window.Close();

            _initialized = false;
            _shutdown = true;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        ShutdownRuntime();

        Window.Dispose();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}