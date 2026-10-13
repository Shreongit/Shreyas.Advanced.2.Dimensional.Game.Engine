namespace SA2DGE.Engine.Graphics.Buffers;

public abstract class VertexBuffer : IDisposable
{
    private bool _disposed;

    public int VertexCount { get; }

    public int VertexSize { get; }

    public int SizeInBytes =>
        VertexCount * VertexSize;

    public bool IsDisposed =>
        _disposed;

    protected VertexBuffer(
        int vertexCount,
        int vertexSize)
    {
        if (vertexCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(vertexCount));
        }

        if (vertexSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(vertexSize));
        }

        VertexCount = vertexCount;
        VertexSize = vertexSize;
    }

    public void SetData<T>(
        ReadOnlySpan<T> data)
        where T : unmanaged
    {
        ThrowIfDisposed();

        if (data.Length > VertexCount)
        {
            throw new ArgumentException(
                "The supplied data exceeds the vertex buffer capacity.",
                nameof(data));
        }

        OnSetData(data);
    }

    public void Bind()
    {
        ThrowIfDisposed();

        OnBind();
    }

    public void Unbind()
    {
        ThrowIfDisposed();

        OnUnbind();
    }

    protected abstract void OnSetData<T>(
        ReadOnlySpan<T> data)
        where T : unmanaged;

    protected abstract void OnBind();

    protected abstract void OnUnbind();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        OnDispose();

        _disposed = true;
    }

    protected virtual void OnDispose()
    {
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}