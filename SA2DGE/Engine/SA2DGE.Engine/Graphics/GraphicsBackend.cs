using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Math;
using SA2DGE.Engine.Platform.Window;
using SA2DGE.Engine.Graphics.Shaders;
using SA2DGE.Engine.Graphics.Textures;

namespace SA2DGE.Engine.Graphics;

public abstract class GraphicsBackend : IDisposable
{
    private bool _disposed;

    public bool IsInitialized { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }
    
    public GraphicsCommands? Commands { get; protected set; }

    public GraphicsResources? Resources { get; protected set; }
    
    public GraphicsShaders? Shaders { get; protected set; }

    protected GraphicsBackend()
    {
    }

    public void Initialize(Window window)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(window);

        if (IsInitialized)
        {
            throw new InvalidOperationException(
                "The graphics backend is already initialized.");
        }

        ValidateSize(
            window.Width,
            window.Height);

        try
        {
            InitializeBackend(window);

            Width = window.Width;
            Height = window.Height;

            IsInitialized = true;
        }
        catch
        {
            try
            {
                ShutdownBackend();
            }
            catch
            {
                // Preserve the original initialization exception.
            }

            Commands = null;
            Resources = null;
            Shaders = null;

            Width = 0;
            Height = 0;
            IsInitialized = false;

            throw;
        }
    }

    public void Resize(int width, int height)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        ValidateSize(width, height);

        if (width == Width && height == Height)
            return;

        ResizeBackend(width, height);

        Width = width;
        Height = height;
    }

    public void Clear(Color color)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        ClearBackend(color);
    }

    public void Present()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        PresentBackend();
    }

    public VertexBuffer CreateVertexBuffer(
        int vertexCount,
        int vertexSize)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Resources is null)
        {
            throw new InvalidOperationException(
                "Graphics resources are not available.");
        }

        return Resources.CreateVertexBuffer(
            vertexCount,
            vertexSize);
    }
    
    public Shader CreateVertexShader(string source)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Shaders is null)
        {
            throw new InvalidOperationException(
                "Graphics shaders are not available.");
        }

        return Shaders.CreateVertexShader(source);
    }
    public Texture2D CreateTexture2D(
        int width,
        int height,
        TextureFormat format,
        ReadOnlySpan<byte> data = default)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Resources is null)
        {
            throw new InvalidOperationException(
                "Graphics resources are not available.");
        }

        return Resources.CreateTexture2D(
            width,
            height,
            format,
            data);
    }
    

    public Shader CreateFragmentShader(string source)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Shaders is null)
        {
            throw new InvalidOperationException(
                "Graphics shaders are not available.");
        }

        return Shaders.CreateFragmentShader(source);
    }

    public ShaderProgram CreateShaderProgram()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Shaders is null)
        {
            throw new InvalidOperationException(
                "Graphics shaders are not available.");
        }

        return Shaders.CreateShaderProgram();
    }

    public IndexBuffer CreateIndexBuffer(
        int indexCount,
        int indexSizeInBytes = sizeof(uint))
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Resources is null)
        {
            throw new InvalidOperationException(
                "Graphics resources are not available.");
        }

        return Resources.CreateIndexBuffer(
            indexCount,
            indexSizeInBytes);
    }

    public VertexArray CreateVertexArray()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (Resources is null)
        {
            throw new InvalidOperationException(
                "Graphics resources are not available.");
        }

        return Resources.CreateVertexArray();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (IsInitialized)
        {
            try
            {
                ShutdownBackend();
            }
            finally
            {
                IsInitialized = false;
            }
        }

        Resources = null;
        _disposed = true;
    }

    protected abstract void InitializeBackend(Window window);
    
    

    protected abstract void ResizeBackend(
        int width,
        int height);

    protected abstract void ClearBackend(Color color);

    protected abstract void PresentBackend();

    protected abstract void ShutdownBackend();

    protected void EnsureInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "The graphics backend has not been initialized.");
            
            
        }
    }

    private static void ValidateSize(
        int width,
        int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                "Width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                "Height must be greater than zero.");
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}