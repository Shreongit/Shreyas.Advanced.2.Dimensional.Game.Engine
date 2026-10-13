using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SA2DGE.Engine.Graphics.Buffers;

internal sealed class Direct3D11InputLayout : IDisposable
{
    private readonly ID3D11Device _device;

    private ID3D11InputLayout? _inputLayout;
    private bool _disposed;

    public bool IsCreated =>
        _inputLayout is not null;

    public Direct3D11InputLayout(
        ID3D11Device device,
        VertexLayout layout,
        ReadOnlySpan<byte> vertexShaderBytecode)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(layout);

        if (layout.IsEmpty)
        {
            throw new ArgumentException(
                "Vertex layout must contain at least one attribute.",
                nameof(layout));
        }

        if (vertexShaderBytecode.IsEmpty)
        {
            throw new ArgumentException(
                "Vertex shader bytecode cannot be empty.",
                nameof(vertexShaderBytecode));
        }

        _device = device;

        InputElementDescription[] elements =
            CreateInputElements(layout);

        _inputLayout =
            _device.CreateInputLayout(
                elements,
                vertexShaderBytecode);
    }

    public void Bind(
        ID3D11DeviceContext context)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(context);

        if (_inputLayout is null)
        {
            throw new InvalidOperationException(
                "The D3D11 input layout has not been created.");
        }

        context.IASetInputLayout(
            _inputLayout);
    }

    public void Unbind(
        ID3D11DeviceContext context)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(context);

        context.IASetInputLayout(
            null);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _inputLayout?.Dispose();
        _inputLayout = null;

        _disposed = true;
    }

    private static InputElementDescription[] CreateInputElements(
        VertexLayout layout)
    {
        InputElementDescription[] elements =
            new InputElementDescription[
                layout.Attributes.Count];

        for (int index = 0;
             index < layout.Attributes.Count;
             index++)
        {
            VertexAttribute attribute =
                layout.Attributes[index];

            elements[index] =
                new InputElementDescription(
                    attribute.SemanticName,
                    checked((uint)attribute.SemanticIndex),
                    GetFormat(attribute.Type),
                    checked((uint)attribute.Offset),
                    0,
                    attribute.PerInstance
                        ? InputClassification.PerInstanceData
                        : InputClassification.PerVertexData,
                    attribute.PerInstance ? 1u : 0u);
        }

        return elements;
    }

    private static Format GetFormat(
        VertexAttributeType type)
    {
        return type switch
        {
            VertexAttributeType.Float =>
                Format.R32_Float,

            VertexAttributeType.Float2 =>
                Format.R32G32_Float,

            VertexAttributeType.Float3 =>
                Format.R32G32B32_Float,

            VertexAttributeType.Float4 =>
                Format.R32G32B32A32_Float,

            VertexAttributeType.Int =>
                Format.R32_SInt,

            VertexAttributeType.Int2 =>
                Format.R32G32_SInt,

            VertexAttributeType.Int3 =>
                Format.R32G32B32_SInt,

            VertexAttributeType.Int4 =>
                Format.R32G32B32A32_SInt,

            VertexAttributeType.UInt =>
                Format.R32_UInt,

            VertexAttributeType.UInt2 =>
                Format.R32G32_UInt,

            VertexAttributeType.UInt3 =>
                Format.R32G32B32_UInt,

            VertexAttributeType.UInt4 =>
                Format.R32G32B32A32_UInt,

            _ => throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Unsupported vertex attribute type.")
        };
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}