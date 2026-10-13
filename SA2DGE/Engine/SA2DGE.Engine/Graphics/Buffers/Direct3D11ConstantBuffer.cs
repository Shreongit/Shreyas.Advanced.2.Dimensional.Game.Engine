using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics.Buffers;

internal sealed class Direct3D11ConstantBuffer : ConstantBuffer
{
    private readonly ID3D11Device _device;
    private readonly ID3D11Buffer _buffer;

    public ID3D11Buffer NativeBuffer => _buffer;

    public Direct3D11ConstantBuffer(
        ID3D11Device device,
        int sizeInBytes)
        : base(AlignTo16(sizeInBytes))
    {
        ArgumentNullException.ThrowIfNull(device);

        _device = device;

        BufferDescription description = new()
        {
            ByteWidth = checked((uint)SizeInBytes),
            Usage = ResourceUsage.Dynamic,
            BindFlags = BindFlags.ConstantBuffer,
            CPUAccessFlags = CpuAccessFlags.Write
        };

        _buffer = _device.CreateBuffer(
            in description,
            (Vortice.Direct3D11.SubresourceData?)null);
    }

    protected override unsafe void OnSetData(ReadOnlySpan<byte> data)
    {
        ID3D11DeviceContext context = _device.ImmediateContext;

        MappedSubresource mapped = context.Map(
            _buffer,
            0,
            MapMode.WriteDiscard,
            Vortice.Direct3D11.MapFlags.None);

        try
        {
            Span<byte> destination = new(
                (void*)mapped.DataPointer,
                SizeInBytes);

            destination.Clear();
            data.CopyTo(destination);
        }
        finally
        {
            context.Unmap(_buffer, 0);
        }
    }

    protected override void OnDispose()
    {
        _buffer.Dispose();
    }

    private static int AlignTo16(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        return checked((size + 15) & ~15);
    }
}