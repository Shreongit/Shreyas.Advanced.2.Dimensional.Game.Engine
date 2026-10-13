using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Materials;

public readonly struct MaterialProperty : IEquatable<MaterialProperty>
{
    public string Name { get; }

    public MaterialPropertyType Type { get; }

    public object Value { get; }

    public MaterialProperty(
        string name,
        MaterialPropertyType type,
        object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Type = type;
        Value = value;
    }

    public static MaterialProperty Int(
        string name,
        int value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Int,
            value);
    }

    public static MaterialProperty Float(
        string name,
        float value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Float,
            value);
    }

    public static MaterialProperty Vector2(
        string name,
        Math.Vector2 value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Vector2,
            value);
    }

    public static MaterialProperty Vector3(
        string name,
        Math.Vector3 value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Vector3,
            value);
    }

    public static MaterialProperty Color(
        string name,
        Math.Color value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Color,
            value);
    }

    public static MaterialProperty Matrix3(
        string name,
        Math.Matrix3 value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Matrix3,
            value);
    }

    public static MaterialProperty Matrix4(
        string name,
        Math.Matrix4 value)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Matrix4,
            value);
    }

    public static MaterialProperty Texture(
        string name,
        object texture)
    {
        return new MaterialProperty(
            name,
            MaterialPropertyType.Texture,
            texture);
    }

    public bool Equals(MaterialProperty other)
    {
        return Name == other.Name &&
               Type == other.Type &&
               Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is MaterialProperty other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Name,
            Type,
            Value);
    }

    public static bool operator ==(
        MaterialProperty left,
        MaterialProperty right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        MaterialProperty left,
        MaterialProperty right)
    {
        return !left.Equals(right);
    }
}

public enum MaterialPropertyType
{
    Int,
    Float,
    Vector2,
    Vector3,
    Color,
    Matrix3,
    Matrix4,
    Texture
}