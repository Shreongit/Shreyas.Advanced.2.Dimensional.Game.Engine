using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Textures;
using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics;

internal sealed class Direct3D11GraphicsResources : GraphicsResources
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    public Direct3D11GraphicsResources(
        ID3D11Device device,
        ID3D11DeviceContext context)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(context);

        _device = device;
        _context = context;
    }
    
    public override ConstantBuffer CreateConstantBuffer(
        int sizeInBytes)
    {
        return new Direct3D11ConstantBuffer(
            _device,
            sizeInBytes);
    }

    public override VertexBuffer CreateVertexBuffer(
        int vertexCount,
        int vertexSize)
    {
        return new Direct3D11VertexBuffer(
            _device,
            _context,
            vertexCount,
            vertexSize);
    }

    public override IndexBuffer CreateIndexBuffer(
        int indexCount,
        int indexSizeInBytes = sizeof(uint))
    {
        return new Direct3D11IndexBuffer(
            _device,
            _context,
            indexCount,
            indexSizeInBytes);
    }

    public override VertexArray CreateVertexArray()
    {
        return new Direct3D11VertexArray(
            _device,
            _context);
    }

    public override Texture2D CreateTexture2D(
        int width,
        int height,
        TextureFormat format,
        ReadOnlySpan<byte> data = default)
    {
        return new Direct3D11Texture2D(
            _device,
            _context,
            width,
            height,
            format,
            data);
    }
}