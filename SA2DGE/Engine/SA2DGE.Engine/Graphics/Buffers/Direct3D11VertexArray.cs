using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics.Buffers;

internal sealed class Direct3D11VertexArray : VertexArray
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    private Direct3D11InputLayout? _inputLayout;
    private VertexLayout? _layout;

    public Direct3D11VertexArray(
        ID3D11Device device,
        ID3D11DeviceContext context)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(context);

        _device = device;
        _context = context;
    }

    protected override void SetLayoutBackend(
        VertexLayout layout,
        ReadOnlySpan<byte> vertexShaderBytecode)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(layout);

        if (layout.IsEmpty)
            throw new ArgumentException(
                "Vertex layout must contain at least one attribute.",
                nameof(layout));

        if (vertexShaderBytecode.IsEmpty)
            throw new ArgumentException(
                "Vertex shader bytecode cannot be empty.",
                nameof(vertexShaderBytecode));

        _inputLayout?.Dispose();

        _inputLayout = new Direct3D11InputLayout(
            _device,
            layout,
            vertexShaderBytecode);

        _layout = layout;
    }

    protected override void OnVertexBufferAdded(VertexBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (_layout is null)
            return;

        if (buffer.VertexSize != _layout.Stride)
        {
            throw new InvalidOperationException(
                $"Vertex buffer stride ({buffer.VertexSize}) does not match " +
                $"vertex layout stride ({_layout.Stride}).");
        }
    }

    protected override void OnIndexBufferSet(IndexBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
    }

    protected override void OnBind()
    {
        if (_inputLayout is null)
        {
            throw new InvalidOperationException(
                "A vertex input layout must be configured before binding.");
        }

        _inputLayout.Bind(_context);

        foreach (VertexBuffer vertexBuffer in VertexBuffers)
            vertexBuffer.Bind();

        IndexBuffer?.Bind();

        _context.IASetPrimitiveTopology(
            PrimitiveTopology.TriangleList);
    }

    protected override void OnUnbind()
    {
        IndexBuffer?.Unbind();

        for (int index = VertexBuffers.Count - 1; index >= 0; index--)
            VertexBuffers[index].Unbind();

        _inputLayout?.Unbind(_context);

        _context.IASetPrimitiveTopology(
            PrimitiveTopology.Undefined);
    }

    protected override void OnDispose()
    {
        _inputLayout?.Dispose();
        _inputLayout = null;
        _layout = null;
    }
}