namespace SA2DGE.Engine.Graphics.Shaders;

public sealed class ShaderSource
{
    public string VertexSource { get; }

    public string FragmentSource { get; }

    public string? GeometrySource { get; }

    public ShaderSource(
        string vertexSource,
        string fragmentSource,
        string? geometrySource = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vertexSource);
        ArgumentException.ThrowIfNullOrWhiteSpace(fragmentSource);

        VertexSource = vertexSource;
        FragmentSource = fragmentSource;
        GeometrySource = geometrySource;
    }

    public static ShaderSource FromFiles(
        string vertexPath,
        string fragmentPath,
        string? geometryPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vertexPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(fragmentPath);

        if (!File.Exists(vertexPath))
        {
            throw new FileNotFoundException(
                "Vertex shader file was not found.",
                vertexPath);
        }

        if (!File.Exists(fragmentPath))
        {
            throw new FileNotFoundException(
                "Fragment shader file was not found.",
                fragmentPath);
        }

        string vertexSource =
            File.ReadAllText(vertexPath);

        string fragmentSource =
            File.ReadAllText(fragmentPath);

        string? geometrySource = null;

        if (geometryPath is not null)
        {
            if (!File.Exists(geometryPath))
            {
                throw new FileNotFoundException(
                    "Geometry shader file was not found.",
                    geometryPath);
            }

            geometrySource =
                File.ReadAllText(geometryPath);
        }

        return new ShaderSource(
            vertexSource,
            fragmentSource,
            geometrySource);
    }
}