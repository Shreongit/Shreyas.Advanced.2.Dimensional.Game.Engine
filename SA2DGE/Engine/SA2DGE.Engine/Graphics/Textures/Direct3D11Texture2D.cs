using System.Runtime.InteropServices;
using Vortice.Direct3D11;
using DxgiFormat = Vortice.DXGI.Format;
using Vortice.DXGI;

namespace SA2DGE.Engine.Graphics.Textures;

internal sealed class Direct3D11Texture2D : Texture2D
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    private ID3D11Texture2D? _texture;
    private ID3D11ShaderResourceView? _shaderResourceView;
    private ID3D11SamplerState? _samplerState;

    public Direct3D11Texture2D(
        ID3D11Device device,
        ID3D11DeviceContext context,
        int width,
        int height,
        TextureFormat format,
        ReadOnlySpan<byte> data = default)
        : base(
            width,
            height,
            data,
            format)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(context);

        _device = device;
        _context = context;

        try
        {
            CreateGpuResources();
            CreateSamplerState();
        }
        catch
        {
            ReleaseGpuResources();
            throw;
        }
    }

    
    protected override void OnBind(int slot)
    {
        EnsureGpuResources();

        if (slot < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slot));
        }

        uint shaderSlot = checked((uint)slot);

        _context.PSSetShaderResource(
            shaderSlot,
            _shaderResourceView!);

        _context.PSSetSampler(
            shaderSlot,
            _samplerState);
    }

    protected override void OnUnbind()
    {
        _context.PSSetShaderResource(
            0u,
            null!);

        _context.PSSetSampler(
            0u,
            null!);
    }
    

    protected override void OnSetData<T>(
        ReadOnlySpan<T> data)
    {
        base.OnSetData(data);

        EnsureGpuResources();

        ReadOnlySpan<byte> bytes =
            MemoryMarshal.AsBytes(data);

        UploadData(bytes);
    }

    protected override void OnDispose()
    {
        _context.PSSetShaderResource(
            0u,
            null!);

        _context.PSSetSampler(
            0u,
            null!);

        ReleaseGpuResources();

        base.OnDispose();
    }

    private void CreateGpuResources()
    {
        var description = new Texture2DDescription
        {
            Width = (uint)Width,
            Height = (uint)Height,
            MipLevels = 1,
            ArraySize = 1,
            Format = ToDxgiFormat(Format),
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Dynamic,
            BindFlags = BindFlags.ShaderResource,
            CPUAccessFlags = CpuAccessFlags.Write,
            MiscFlags = ResourceOptionFlags.None
        };

        _texture = _device.CreateTexture2D(description);

        _shaderResourceView =
            _device.CreateShaderResourceView(_texture);

        ReadOnlySpan<byte> data = GetDataSpan();

        if (!data.IsEmpty)
        {
            UploadData(data);
        }
    }

    private void CreateSamplerState()
    {
        var filter = Filter switch
        {
            TextureFilter.Nearest =>
                Vortice.Direct3D11.Filter.MinMagMipPoint,

            TextureFilter.Linear =>
                Vortice.Direct3D11.Filter.MinMagMipLinear,

            _ => throw new ArgumentOutOfRangeException(
                nameof(Filter),
                Filter,
                "Unsupported texture filter.")
        };

        var addressMode = Wrap switch
        {
            TextureWrap.Repeat =>
                TextureAddressMode.Wrap,

            TextureWrap.MirroredRepeat =>
                TextureAddressMode.Mirror,

            TextureWrap.ClampToEdge =>
                TextureAddressMode.Clamp,

            TextureWrap.ClampToBorder =>
                TextureAddressMode.Border,

            _ => throw new ArgumentOutOfRangeException(
                nameof(Wrap),
                Wrap,
                "Unsupported texture wrap mode.")
        };

        var description = new SamplerDescription
        {
            Filter = filter,
            AddressU = addressMode,
            AddressV = addressMode,
            AddressW = addressMode,
            MipLODBias = 0.0f,
            MaxAnisotropy = 1,
            ComparisonFunc = ComparisonFunction.Never,
            BorderColor = new Vortice.Mathematics.Color4(
                0.0f,
                0.0f,
                0.0f,
                0.0f),
            MinLOD = 0.0f,
            MaxLOD = float.MaxValue
        };

        _samplerState =
            _device.CreateSamplerState(description);
    }

    private void UploadData(ReadOnlySpan<byte> data)
    {
        int sourceBytesPerPixel = BytesPerPixel;
        int gpuBytesPerPixel = GetGpuBytesPerPixel();

        int expectedSourceSize =
            checked(Width * Height * sourceBytesPerPixel);

        if (data.Length != expectedSourceSize)
        {
            throw new ArgumentException(
                $"Texture data must contain exactly " +
                $"{expectedSourceSize} bytes.",
                nameof(data));
        }

        int gpuRowPitch =
            checked(Width * gpuBytesPerPixel);

        byte[]? convertedData = null;

        ReadOnlySpan<byte> uploadData = data;

        if (Format ==
            SA2DGE.Engine.Graphics.Textures.TextureFormat.RGB8)
        {
            convertedData =
                new byte[checked(Width * Height * 4)];

            for (
                int source = 0, destination = 0;
                source < data.Length;
                source += 3, destination += 4)
            {
                convertedData[destination] =
                    data[source];

                convertedData[destination + 1] =
                    data[source + 1];

                convertedData[destination + 2] =
                    data[source + 2];

                convertedData[destination + 3] =
                    byte.MaxValue;
            }

            uploadData = convertedData;
        }

        var mapped = _context.Map(
            _texture!,
            0,
            MapMode.WriteDiscard);

        try
        {
            unsafe
            {
                fixed (byte* source = uploadData)
                {
                    byte* destination =
                        (byte*)mapped.DataPointer;

                    for (int y = 0; y < Height; y++)
                    {
                        Buffer.MemoryCopy(
                            source + (y * gpuRowPitch),
                            destination + (y * mapped.RowPitch),
                            mapped.RowPitch,
                            gpuRowPitch);
                    }
                }
            }
        }
        finally
        {
            _context.Unmap(
                _texture!,
                0);
        }
    }

    private void ReleaseGpuResources()
    {
        _samplerState?.Dispose();
        _samplerState = null;

        _shaderResourceView?.Dispose();
        _shaderResourceView = null;

        _texture?.Dispose();
        _texture = null;
    }

    private void EnsureGpuResources()
    {
        if (_texture is null ||
            _shaderResourceView is null ||
            _samplerState is null)
        {
            throw new ObjectDisposedException(
                nameof(Direct3D11Texture2D));
        }
    }

    private int GetGpuBytesPerPixel()
    {
        return Format switch
        {
            SA2DGE.Engine.Graphics.Textures.TextureFormat.R8 => 1,
            SA2DGE.Engine.Graphics.Textures.TextureFormat.RG8 => 2,
            SA2DGE.Engine.Graphics.Textures.TextureFormat.RGB8 => 4,
            SA2DGE.Engine.Graphics.Textures.TextureFormat.RGBA8 => 4,

            _ => throw new ArgumentOutOfRangeException(
                nameof(Format),
                Format,
                "Unsupported texture format.")
        };
    }

    private static DxgiFormat ToDxgiFormat(
        TextureFormat format)
    {
        return format switch
        {
            SA2DGE.Engine.Graphics.Textures.TextureFormat.R8 =>
                DxgiFormat.R8_UNorm,

            SA2DGE.Engine.Graphics.Textures.TextureFormat.RG8 =>
                DxgiFormat.R8G8_UNorm,

            SA2DGE.Engine.Graphics.Textures.TextureFormat.RGB8 =>
                DxgiFormat.R8G8B8A8_UNorm,

            SA2DGE.Engine.Graphics.Textures.TextureFormat.RGBA8 =>
                DxgiFormat.R8G8B8A8_UNorm,

            _ => throw new ArgumentOutOfRangeException(
                nameof(format),
                format,
                "Unsupported texture format.")
        };
    }
}