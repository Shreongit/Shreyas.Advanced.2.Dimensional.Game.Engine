using SA2DGE.Engine.Graphics.Shaders;

namespace SA2DGE.Engine.Graphics;

public abstract class GraphicsShaders
{
    public abstract Shader CreateVertexShader(string source);

    public abstract Shader CreateFragmentShader(string source);

    public abstract ShaderProgram CreateShaderProgram();
}