namespace SA2DGE.Engine.Graphics.Buffers;

public abstract class IndexBuffer : IDisposable
{
    private bool _disposed;

    public int IndexCount { get; }

    public int IndexSizeInBytes { get; }

    public bool IsDisposed =>
        _disposed;

    protected IndexBuffer(
        int indexCount,
        int indexSizeInBytes = sizeof(uint))
    {
        if (indexCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(indexCount));
        }

        if (indexSizeInBytes != sizeof(ushort) &&
            indexSizeInBytes != sizeof(uint))
        {
            throw new ArgumentException(
                "Index size must be 2 or 4 bytes.",
                nameof(indexSizeInBytes));
        }

        IndexCount = indexCount;
        IndexSizeInBytes = indexSizeInBytes;
    }

    public void SetData<T>(
        ReadOnlySpan<T> data)
        where T : unmanaged
    {
        ThrowIfDisposed();

        if (data.Length > IndexCount)
        {
            throw new ArgumentException(
                "The supplied data exceeds the index buffer capacity.",
                nameof(data));
        }

        int elementSize =
            System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

        if (elementSize != IndexSizeInBytes)
        {
            throw new ArgumentException(
                $"Index element size must be {IndexSizeInBytes} bytes.",
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