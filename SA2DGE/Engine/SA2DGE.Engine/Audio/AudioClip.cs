namespace SA2DGE.Engine.Audio;

public sealed class AudioClip : IDisposable
{
    private byte[] _data;
    private bool _disposed;

    public string Name { get; }

    public int SampleRate { get; }

    public int Channels { get; }

    public int BitsPerSample { get; }

    public int DataSizeInBytes =>
        _data.Length;

    public int BytesPerSample =>
        BitsPerSample / 8;

    public int BytesPerSecond =>
        SampleRate *
        Channels *
        BytesPerSample;

    public float Duration =>
        BytesPerSecond > 0
            ? (float)_data.Length / BytesPerSecond
            : 0.0f;

    public bool IsDisposed =>
        _disposed;

    public AudioClip(
        string name,
        int sampleRate,
        int channels,
        int bitsPerSample,
        ReadOnlySpan<byte> data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (sampleRate <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sampleRate));
        }

        if (channels <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(channels));
        }

        if (bitsPerSample <= 0 ||
            bitsPerSample % 8 != 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bitsPerSample));
        }

        Name = name;
        SampleRate = sampleRate;
        Channels = channels;
        BitsPerSample = bitsPerSample;

        _data = data.ToArray();
    }

    public ReadOnlySpan<byte> GetData()
    {
        ThrowIfDisposed();

        return _data;
    }

    public byte[] GetDataCopy()
    {
        ThrowIfDisposed();

        return (byte[])_data.Clone();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _data = Array.Empty<byte>();
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
        return $"{Name} ({Duration:0.###}s, " +
               $"{SampleRate}Hz, {Channels}ch)";
    }
}