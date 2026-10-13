using Vortice.Direct3D11;

namespace SA2DGE.Engine.Graphics.Shaders;

internal sealed class Direct3D11ShaderProgram : ShaderProgram
{
    private readonly ID3D11DeviceContext _context;

    private Direct3D11VertexShader? _vertexShader;
    private Direct3D11FragmentShader? _fragmentShader;

    

    public Direct3D11ShaderProgram(
        ID3D11DeviceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    protected override void OnAttach(
        Shader shader)
    {
        switch (shader)
        {
            case Direct3D11VertexShader vertexShader:
                if (_vertexShader is not null)
                {
                    throw new InvalidOperationException(
                        "A vertex shader is already attached.");
                }

                _vertexShader = vertexShader;
                break;

            case Direct3D11FragmentShader fragmentShader:
                if (_fragmentShader is not null)
                {
                    throw new InvalidOperationException(
                        "A fragment shader is already attached.");
                }

                _fragmentShader = fragmentShader;
                break;

            default:
                throw new ArgumentException(
                    "The shader must be a Direct3D 11 shader.",
                    nameof(shader));
        }

        
    }

    protected override void OnLink()
    {
        if (_vertexShader is null)
        {
            throw new InvalidOperationException(
                "A vertex shader must be attached before linking.");
        }

        if (_fragmentShader is null)
        {
            throw new InvalidOperationException(
                "A fragment shader must be attached before linking.");
        }

        if (!_vertexShader.IsCompiled)
        {
            throw new InvalidOperationException(
                "The attached vertex shader has not been compiled.");
        }

        if (!_fragmentShader.IsCompiled)
        {
            throw new InvalidOperationException(
                "The attached fragment shader has not been compiled.");
        }

        
    }

    protected override void OnUse()
    {
        EnsureD3D11Linked();

        _vertexShader!.Bind(_context);
        _fragmentShader!.Bind(_context);
    }

    protected override void OnStopUsing()
    {
        _context.VSSetShader(
            null,
            null,
            0);

        _context.PSSetShader(
            null,
            null,
            0);
    }

    protected override int OnGetUniformLocation(
        string name)
    {
        throw new NotSupportedException(
            "D3D11 does not use OpenGL-style uniform locations. " +
            "Use constant-buffer resources for shader parameters.");
    }

    protected override void OnSetInt(
        int location,
        int value)
    {
        throw CreateUniformException();
    }

    protected override void OnSetFloat(
        int location,
        float value)
    {
        throw CreateUniformException();
    }

    protected override void OnSetVector2(
        int location,
        SA2DGE.Engine.Math.Vector2 value)
    {
        throw CreateUniformException();
    }

    protected override void OnSetVector3(
        int location,
        SA2DGE.Engine.Math.Vector3 value)
    {
        throw CreateUniformException();
    }

    protected override void OnSetColor(
        int location,
        SA2DGE.Engine.Math.Color value)
    {
        throw CreateUniformException();
    }

    protected override void OnSetMatrix3(
        int location,
        SA2DGE.Engine.Math.Matrix3 value)
    {
        throw CreateUniformException();
    }

    protected override void OnSetMatrix4(
        int location,
        SA2DGE.Engine.Math.Matrix4 value)
    {
        throw CreateUniformException();
    }

    protected override void OnDispose()
    {
        _vertexShader = null;
        _fragmentShader = null;
        
    }

    private void EnsureD3D11Linked()
    {
        if (!IsLinked ||
            _vertexShader is null ||
            _fragmentShader is null)
        {
            throw new InvalidOperationException(
                "The Direct3D 11 shader program has not been linked.");
        }
    }

    private static NotSupportedException CreateUniformException()
    {
        return new NotSupportedException(
            "D3D11 shader uniforms are implemented through " +
            "constant buffers rather than uniform locations.");
    }
}