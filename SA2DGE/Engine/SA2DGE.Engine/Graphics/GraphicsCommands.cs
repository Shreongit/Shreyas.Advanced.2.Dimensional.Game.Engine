using SA2DGE.Engine.Graphics.Buffers;
using SA2DGE.Engine.Graphics.Shaders;

namespace SA2DGE.Engine.Graphics;

public abstract class GraphicsCommands
{
    private bool _disposed;

    public bool IsDisposed => _disposed;

    public void SetVertexArray(VertexArray vertexArray)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(vertexArray);

        SetVertexArrayBackend(vertexArray);
    }
    
    public abstract void SetVertexConstantBuffer(
        int slot,
        ConstantBuffer buffer);
    
    public abstract void SetPixelConstantBuffer(
        int slot,
        ConstantBuffer buffer);

    public void SetShaderProgram(ShaderProgram shaderProgram)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(shaderProgram);

        SetShaderProgramBackend(shaderProgram);
    }

    public void DrawIndexed(int indexCount, int startIndex = 0)
    {
        ThrowIfDisposed();

        if (indexCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(indexCount));

        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        DrawIndexedBackend(indexCount, startIndex);
    }

    public void Draw(int vertexCount, int startVertex = 0)
    {
        ThrowIfDisposed();

        if (vertexCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(vertexCount));

        if (startVertex < 0)
            throw new ArgumentOutOfRangeException(nameof(startVertex));

        DrawBackend(vertexCount, startVertex);
    }

    public void Reset()
    {
        ThrowIfDisposed();

        ResetBackend();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        DisposeBackend();

        _disposed = true;
    }

    protected abstract void SetVertexArrayBackend(
        VertexArray vertexArray);

    protected abstract void SetShaderProgramBackend(
        ShaderProgram shaderProgram);

    protected abstract void DrawIndexedBackend(
        int indexCount,
        int startIndex);

    protected abstract void DrawBackend(
        int vertexCount,
        int startVertex);

    protected abstract void ResetBackend();

    protected virtual void DisposeBackend()
    {
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}