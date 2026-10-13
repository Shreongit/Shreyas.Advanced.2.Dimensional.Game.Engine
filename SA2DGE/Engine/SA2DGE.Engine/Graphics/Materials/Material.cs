using SA2DGE.Engine.Graphics.Shaders;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Materials;

public sealed class Material : IDisposable
{
    private readonly Dictionary<string, MaterialProperty> _properties = new();

    private bool _disposed;

    public ShaderProgram Shader { get; }

    public IReadOnlyDictionary<string, MaterialProperty> Properties =>
        _properties;

    public Material(ShaderProgram shader)
    {
        ArgumentNullException.ThrowIfNull(shader);

        Shader = shader;
    }

    public void Set(
        MaterialProperty property)
    {
        ThrowIfDisposed();

        _properties[property.Name] = property;
    }

    public void SetInt(
        string name,
        int value)
    {
        Set(MaterialProperty.Int(name, value));
    }

    public void SetFloat(
        string name,
        float value)
    {
        Set(MaterialProperty.Float(name, value));
    }

    public void SetVector2(
        string name,
        Vector2 value)
    {
        Set(MaterialProperty.Vector2(name, value));
    }

    public void SetVector3(
        string name,
        Vector3 value)
    {
        Set(MaterialProperty.Vector3(name, value));
    }

    public void SetColor(
        string name,
        Color value)
    {
        Set(MaterialProperty.Color(name, value));
    }

    public void SetMatrix3(
        string name,
        Matrix3 value)
    {
        Set(MaterialProperty.Matrix3(name, value));
    }

    public void SetMatrix4(
        string name,
        Matrix4 value)
    {
        Set(MaterialProperty.Matrix4(name, value));
    }

    public void SetTexture(
        string name,
        object texture)
    {
        Set(MaterialProperty.Texture(name, texture));
    }

    public bool Has(string name)
    {
        return !_disposed &&
               _properties.ContainsKey(name);
    }

    public bool TryGet(
        string name,
        out MaterialProperty property)
    {
        if (_disposed)
        {
            property = default;
            return false;
        }

        return _properties.TryGetValue(
            name,
            out property);
    }

    public void Apply()
    {
        ThrowIfDisposed();

        Shader.Use();

        foreach (MaterialProperty property in _properties.Values)
        {
            ApplyProperty(property);
        }
    }

    public void Clear()
    {
        ThrowIfDisposed();

        _properties.Clear();
    }

    private void ApplyProperty(
        MaterialProperty property)
    {
        switch (property.Type)
        {
            case MaterialPropertyType.Int:
                Shader.SetInt(
                    property.Name,
                    (int)property.Value);
                break;

            case MaterialPropertyType.Float:
                Shader.SetFloat(
                    property.Name,
                    (float)property.Value);
                break;

            case MaterialPropertyType.Vector2:
                Shader.SetVector2(
                    property.Name,
                    (Vector2)property.Value);
                break;

            case MaterialPropertyType.Vector3:
                Shader.SetVector3(
                    property.Name,
                    (Vector3)property.Value);
                break;

            case MaterialPropertyType.Color:
                Shader.SetColor(
                    property.Name,
                    (Color)property.Value);
                break;

            case MaterialPropertyType.Matrix3:
                Shader.SetMatrix3(
                    property.Name,
                    (Matrix3)property.Value);
                break;

            case MaterialPropertyType.Matrix4:
                Shader.SetMatrix4(
                    property.Name,
                    (Matrix4)property.Value);
                break;

            case MaterialPropertyType.Texture:
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported material property type: {property.Type}.");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _properties.Clear();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}