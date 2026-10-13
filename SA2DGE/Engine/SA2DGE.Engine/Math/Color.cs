namespace SA2DGE.Engine.Math;

public readonly struct Color : IEquatable<Color>
{
    public float R { get; }

    public float G { get; }

    public float B { get; }

    public float A { get; }

    public static Color White => new(1.0f, 1.0f, 1.0f, 1.0f);

    public static Color Black => new(0.0f, 0.0f, 0.0f, 1.0f);

    public static Color Red => new(1.0f, 0.0f, 0.0f, 1.0f);

    public static Color Green => new(0.0f, 1.0f, 0.0f, 1.0f);

    public static Color Blue => new(0.0f, 0.0f, 1.0f, 1.0f);

    public static Color Transparent =>
        new(0.0f, 0.0f, 0.0f, 0.0f);

    public Color(
        float r,
        float g,
        float b,
        float a = 1.0f)
    {
        R = System.Math.Clamp(r, 0.0f, 1.0f);
        G = System.Math.Clamp(g, 0.0f, 1.0f);
        B = System.Math.Clamp(b, 0.0f, 1.0f);
        A = System.Math.Clamp(a, 0.0f, 1.0f);
    }

    public Color(
        byte r,
        byte g,
        byte b,
        byte a = 255)
        : this(
            r / 255.0f,
            g / 255.0f,
            b / 255.0f,
            a / 255.0f)
    {
    }

    public static Color FromHex(uint hex)
    {
        byte r = (byte)((hex >> 16) & 0xFF);
        byte g = (byte)((hex >> 8) & 0xFF);
        byte b = (byte)(hex & 0xFF);

        return new Color(r, g, b);
    }

    public static Color FromHexRgba(uint hex)
    {
        byte r = (byte)((hex >> 24) & 0xFF);
        byte g = (byte)((hex >> 16) & 0xFF);
        byte b = (byte)((hex >> 8) & 0xFF);
        byte a = (byte)(hex & 0xFF);

        return new Color(r, g, b, a);
    }

    public uint ToRgba()
    {
        uint r = (uint)(R * 255.0f);
        uint g = (uint)(G * 255.0f);
        uint b = (uint)(B * 255.0f);
        uint a = (uint)(A * 255.0f);

        return
            (r << 24) |
            (g << 16) |
            (b << 8) |
            a;
    }

    public static Color Lerp(
        Color a,
        Color b,
        float amount)
    {
        return new Color(
            a.R + (b.R - a.R) * amount,
            a.G + (b.G - a.G) * amount,
            a.B + (b.B - a.B) * amount,
            a.A + (b.A - a.A) * amount);
    }

    public static Color operator *(
        Color color,
        float value)
    {
        return new Color(
            color.R * value,
            color.G * value,
            color.B * value,
            color.A);
    }

    public bool Equals(Color other)
    {
        return R.Equals(other.R) &&
               G.Equals(other.G) &&
               B.Equals(other.B) &&
               A.Equals(other.A);
    }

    public override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    public static bool operator ==(
        Color left,
        Color right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Color left,
        Color right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"({R}, {G}, {B}, {A})";
    }
}