using System.Runtime.CompilerServices;
using SA2DGE.Engine.Graphics;
using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics.Buffers;

internal sealed class Direct3D11VertexBuffer : VertexBuffer
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    private ID3D11Buffer? _buffer;

    public Direct3D11VertexBuffer(
        ID3D11Device device,
        ID3D11DeviceContext context,
        int vertexCount,
        int vertexSize)
        : base(vertexCount, vertexSize)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(context);

        _device = device;
        _context = context;

        BufferDescription description = new()
        {
            ByteWidth = checked((uint)SizeInBytes),
            Usage = ResourceUsage.Dynamic,
            BindFlags = BindFlags.VertexBuffer,
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
                "The Direct3D 11 vertex buffer has not been initialized.");
        }

        int dataSize =
            checked(data.Length *
                    Unsafe.SizeOf<T>());

        if (dataSize > SizeInBytes)
        {
            throw new ArgumentException(
                "The supplied data exceeds the vertex buffer capacity.",
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
                "The Direct3D 11 vertex buffer has not been initialized.");
        }

        _context.IASetVertexBuffers(
            0,
            new[]
            {
                _buffer
            },
            new uint[]
            {
                checked((uint)VertexSize)
            },
            new uint[]
            {
                0
            });
    }

    protected override void OnUnbind()
    {
        _context.IASetVertexBuffers(
            0,
            new ID3D11Buffer[]
            {
                null!
            },
            new uint[]
            {
                0
            },
            new uint[]
            {
                0
            });
    }

    protected override void OnDispose()
    {
        _buffer?.Dispose();
        _buffer = null;
    }
}