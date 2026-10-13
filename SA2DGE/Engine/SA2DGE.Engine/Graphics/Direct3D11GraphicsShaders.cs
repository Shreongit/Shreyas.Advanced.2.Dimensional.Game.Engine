using SA2DGE.Engine.Graphics.Shaders;
using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics;

internal sealed class Direct3D11GraphicsShaders : GraphicsShaders
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    public Direct3D11GraphicsShaders(
        ID3D11Device device,
        ID3D11DeviceContext context)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(context);

        _device = device;
        _context = context;
    }

    public override Shader CreateVertexShader(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        return new Direct3D11VertexShader(
            _device,
            source);
    }

    public override Shader CreateFragmentShader(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        return new Direct3D11FragmentShader(
            _device,
            source);
    }

    public override ShaderProgram CreateShaderProgram()
    {
        return new Direct3D11ShaderProgram(
            _context);
    }
}