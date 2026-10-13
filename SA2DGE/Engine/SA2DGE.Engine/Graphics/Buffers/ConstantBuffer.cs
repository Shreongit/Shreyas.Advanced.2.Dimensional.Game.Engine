namespace SA2DGE.Engine.Graphics.Buffers;

public abstract class ConstantBuffer : IDisposable
{
    public int SizeInBytes { get; }

    public bool IsDisposed { get; private set; }

    protected ConstantBuffer(int sizeInBytes)
    {
        if (sizeInBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sizeInBytes),
                "Constant buffer size must be greater than zero.");
        }

        SizeInBytes = sizeInBytes;
    }

    public void SetData(ReadOnlySpan<byte> data)
    {
        ThrowIfDisposed();

        if (data.Length > SizeInBytes)
        {
            throw new ArgumentException(
                "The supplied data is larger than the constant buffer.",
                nameof(data));
        }

        OnSetData(data);
    }

    protected abstract void OnSetData(ReadOnlySpan<byte> data);

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        OnDispose();
        IsDisposed = true;
    }

    protected virtual void OnDispose()
    {
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
    }
}