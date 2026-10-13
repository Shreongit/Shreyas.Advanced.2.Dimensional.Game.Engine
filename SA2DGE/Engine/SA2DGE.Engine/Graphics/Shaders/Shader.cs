namespace SA2DGE.Engine.Graphics.Shaders;



public abstract class Shader : IDisposable
{
    private bool _disposed;

    public ShaderType Type { get; }

    public bool IsCompiled { get; private set; }

    public string Source { get; }
    
    public abstract ReadOnlyMemory<byte> Bytecode { get; }

    protected Shader(
        ShaderType type,
        string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        Type = type;
        Source = source;
    }

    public void Compile()
    {
        ThrowIfDisposed();

        if (IsCompiled)
        {
            return;
        }

        OnCompile();

        IsCompiled = true;
    }

    public void Delete()
    {
        if (_disposed)
        {
            return;
        }

        OnDelete();

        IsCompiled = false;
    }

    protected abstract void OnCompile();

    protected virtual void OnDelete()
    {
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Delete();

        _disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}


public enum ShaderType
{
    Vertex,
    Fragment,
    Geometry,
    Compute
}