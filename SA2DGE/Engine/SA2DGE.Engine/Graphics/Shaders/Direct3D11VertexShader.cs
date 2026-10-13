using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics.Shaders;

internal sealed class Direct3D11VertexShader : Shader
{
    private readonly ID3D11Device _device;

    private ID3D11VertexShader? _shader;

    private ReadOnlyMemory<byte> _bytecode;

    public override ReadOnlyMemory<byte> Bytecode =>
        _bytecode;

    public Direct3D11VertexShader(
        ID3D11Device device,
        string source)
        : base(
            ShaderType.Vertex,
            source)
    {
        ArgumentNullException.ThrowIfNull(device);

        _device = device;
    }

    protected override void OnCompile()
    {
        ReadOnlyMemory<byte> bytecode =
            Compiler.Compile(
                Source,
                "main",
                "SA2DGE_VertexShader.hlsl",
                "vs_5_0");

        if (bytecode.IsEmpty)
        {
            throw new InvalidOperationException(
                "Direct3D 11 vertex shader compilation produced no bytecode.");
        }

        _shader =
            _device.CreateVertexShader(
                bytecode.Span);

        _bytecode = bytecode;
    }

    protected override void OnDelete()
    {
        _shader?.Dispose();
        _shader = null;

        _bytecode =
            ReadOnlyMemory<byte>.Empty;
    }

    internal void Bind(
        ID3D11DeviceContext context)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(context);

        if (_shader is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 vertex shader has not been compiled.");
        }

        context.VSSetShader(
            _shader,
            null,
            0);
    }
}