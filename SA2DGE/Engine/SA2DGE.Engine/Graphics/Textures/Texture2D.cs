using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Textures;

public  class Texture2D : Texture
{
    private byte[] _data;

    public int Channels { get; }

    public TextureFilter Filter { get; set; }

    public TextureWrap Wrap { get; set; }

    public TextureFormat Format { get; }

    public int BytesPerPixel =>
        Channels;

    public int DataSizeInBytes =>
        _data.Length;

    public Texture2D(
        int width,
        int height,
        TextureFormat format = TextureFormat.RGBA8,
        TextureFilter filter = TextureFilter.Linear,
        TextureWrap wrap = TextureWrap.ClampToEdge)
        : base(width, height)
    {
        Format = format;
        Filter = filter;
        Wrap = wrap;
        Channels = GetChannelCount(format);

        _data = new byte[
            width *
            height *
            Channels];
    }

    public Texture2D(
        int width,
        int height,
        ReadOnlySpan<byte> data,
        TextureFormat format = TextureFormat.RGBA8,
        TextureFilter filter = TextureFilter.Linear,
        TextureWrap wrap = TextureWrap.ClampToEdge)
        : this(
            width,
            height,
            format,
            filter,
            wrap)
    {
        if (data.Length != _data.Length)
        {
            throw new ArgumentException(
                $"Texture data must contain exactly {_data.Length} bytes.",
                nameof(data));
        }

        data.CopyTo(_data);
    }

    public void SetPixel(
        int x,
        int y,
        Color color)
    {
        ThrowIfDisposed();

        if ((uint)x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if ((uint)y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (Channels < 3)
        {
            throw new InvalidOperationException(
                "The texture format does not support RGB color data.");
        }

        int index =
            ((y * Width) + x) *
            Channels;

        _data[index] =
            ToByte(color.R);

        _data[index + 1] =
            ToByte(color.G);

        _data[index + 2] =
            ToByte(color.B);

        if (Channels >= 4)
        {
            _data[index + 3] =
                ToByte(color.A);
        }
    }

    public Color GetPixel(
        int x,
        int y)
    {
        ThrowIfDisposed();

        if ((uint)x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if ((uint)y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        int index =
            ((y * Width) + x) *
            Channels;

        float r = _data[index] / 255.0f;

        float g = Channels >= 2
            ? _data[index + 1] / 255.0f
            : r;

        float b = Channels >= 3
            ? _data[index + 2] / 255.0f
            : r;

        float a = Channels >= 4
            ? _data[index + 3] / 255.0f
            : 1.0f;

        return new Color(r, g, b, a);
    }

    public byte[] GetData()
    {
        ThrowIfDisposed();

        return (byte[])_data.Clone();
    }

    public ReadOnlySpan<byte> GetDataSpan()
    {
        ThrowIfDisposed();

        return _data;
    }

    protected override void OnBind(int slot)
    {
    }

    protected override void OnUnbind()
    {
    }

    protected override void OnSetData<T>(
        ReadOnlySpan<T> data)
    {
        int byteCount =
            data.Length *
            System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

        if (byteCount != _data.Length)
        {
            throw new ArgumentException(
                $"Texture data must contain exactly {_data.Length} bytes.",
                nameof(data));
        }

        ReadOnlySpan<byte> bytes =
            System.Runtime.InteropServices.MemoryMarshal.AsBytes(data);

        bytes.CopyTo(_data);
    }

    protected override void OnDispose()
    {
        _data = Array.Empty<byte>();
    }

    private static int GetChannelCount(
        TextureFormat format)
    {
        return format switch
        {
            TextureFormat.R8 => 1,
            TextureFormat.RG8 => 2,
            TextureFormat.RGB8 => 3,
            TextureFormat.RGBA8 => 4,
            _ => throw new ArgumentOutOfRangeException(
                nameof(format))
        };
    }

    private static byte ToByte(float value)
    {
        return (byte)System.Math.Clamp(
            value * 255.0f,
            0.0f,
            255.0f);
    }
}

public enum TextureFormat
{
    R8,
    RG8,
    RGB8,
    RGBA8
}

public enum TextureFilter
{
    Nearest,
    Linear
}

public enum TextureWrap
{
    Repeat,
    MirroredRepeat,
    ClampToEdge,
    ClampToBorder
}