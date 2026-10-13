namespace SA2DGE.Engine.Graphics.Textures;

public abstract class Texture : IDisposable
{
    private bool _disposed;

    public int Width { get; }
    public int Height { get; }

    protected Texture(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(width),
                "Texture width must be greater than zero.");

        if (height <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(height),
                "Texture height must be greater than zero.");

        Width = width;
        Height = height;
    }

    public void Bind(int slot = 0)
    {
        ThrowIfDisposed();

        if (slot < 0)
            throw new ArgumentOutOfRangeException(
                nameof(slot),
                "Texture slot cannot be negative.");

        OnBind(slot);
    }

    public void Unbind()
    {
        ThrowIfDisposed();
        OnUnbind();
    }

    public void SetData<T>(ReadOnlySpan<T> data)
        where T : unmanaged
    {
        ThrowIfDisposed();
        OnSetData(data);
    }

    protected virtual void OnBind(int slot)
    {
    }

    protected virtual void OnUnbind()
    {
    }

    protected virtual void OnSetData<T>(ReadOnlySpan<T> data)
        where T : unmanaged
    {
    }

    protected virtual void OnDispose()
    {
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        OnDispose();

        GC.SuppressFinalize(this);
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}