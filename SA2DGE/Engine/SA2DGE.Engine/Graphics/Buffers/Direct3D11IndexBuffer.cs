using System.Runtime.CompilerServices;
using SA2DGE.Engine.Graphics;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SA2DGE.Engine.Graphics.Buffers;

internal sealed class Direct3D11IndexBuffer : IndexBuffer
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    private ID3D11Buffer? _buffer;

    public Direct3D11IndexBuffer(
        ID3D11Device device,
        ID3D11DeviceContext context,
        int indexCount,
        int indexSizeInBytes = sizeof(uint))
        : base(indexCount, indexSizeInBytes)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(context);

        _device = device;
        _context = context;

        BufferDescription description = new()
        {
            ByteWidth = checked((uint)(
                IndexCount * IndexSizeInBytes)),

            Usage = ResourceUsage.Dynamic,

            BindFlags = BindFlags.IndexBuffer,

            CPUAccessFlags = CpuAccessFlags.Write,

            MiscFlags = ResourceOptionFlags.None,

            StructureByteStride = 0
        };

        _buffer = _device.CreateBuffer(description);
    }

    protected override void OnSetData<T>(
        ReadOnlySpan<T> data)
    {
        if (_buffer is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 index buffer has not been initialized.");
        }

        int dataSize =
            checked(data.Length *
                    Unsafe.SizeOf<T>());

        if (dataSize > IndexCount * IndexSizeInBytes)
        {
            throw new ArgumentException(
                "The supplied data exceeds the index buffer capacity.",
                nameof(data));
        }

        _buffer.SetData(
            _context,
            data,
            MapMode.WriteDiscard);
    }

    protected override void OnBind()
    {
        if (_buffer is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 index buffer has not been initialized.");
        }

        Format format =
            IndexSizeInBytes == sizeof(ushort)
                ? Format.R16_UInt
                : Format.R32_UInt;

        _context.IASetIndexBuffer(
            _buffer,
            format,
            0);
    }

    protected override void OnUnbind()
    {
        _context.IASetIndexBuffer(
            null,
            Format.R32_UInt,
            0);
    }

    protected override void OnDispose()
    {
        _buffer?.Dispose();
        _buffer = null;
    }
}