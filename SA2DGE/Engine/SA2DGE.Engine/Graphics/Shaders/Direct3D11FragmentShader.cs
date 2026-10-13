using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics.Shaders;

internal sealed class Direct3D11FragmentShader : Shader
{
    private readonly ID3D11Device _device;

    private ID3D11PixelShader? _shader;
    private ReadOnlyMemory<byte> _bytecode;

    public override ReadOnlyMemory<byte> Bytecode =>
        _bytecode;

    public Direct3D11FragmentShader(
        ID3D11Device device,
        string source)
        : base(ShaderType.Fragment, source)
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
                "SA2DGE_FragmentShader.hlsl",
                "ps_5_0");

        if (bytecode.IsEmpty)
        {
            throw new InvalidOperationException(
                "D3D11 fragment shader compilation produced no bytecode.");
        }

        _shader =
            _device.CreatePixelShader(
                bytecode.Span);

        _bytecode = bytecode;
    }

    protected override void OnDelete()
    {
        _shader?.Dispose();
        _shader = null;

        _bytecode = ReadOnlyMemory<byte>.Empty;
    }

    internal void Bind(
        ID3D11DeviceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (_shader is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 fragment shader has not been compiled.");
        }

        context.PSSetShader(
            _shader,
            null,
            0);
    }
}