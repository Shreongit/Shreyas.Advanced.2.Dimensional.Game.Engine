using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Textures;

namespace SA2DGE.Engine.Graphics;

public abstract class GraphicsResources
{
    public abstract VertexBuffer CreateVertexBuffer(
        int vertexCount,
        int vertexSize);

    public abstract IndexBuffer CreateIndexBuffer(
        int indexCount,
        int indexSizeInBytes = sizeof(uint));

    public abstract VertexArray CreateVertexArray();
    
    
    public abstract ConstantBuffer CreateConstantBuffer(
        int sizeInBytes);

    public abstract Texture2D CreateTexture2D(
        int width,
        int height,
        TextureFormat format,
        ReadOnlySpan<byte> data = default);
    
}