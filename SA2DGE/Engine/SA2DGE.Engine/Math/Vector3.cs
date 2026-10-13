namespace SA2DGE.Engine.Math;

public readonly struct Vector3 : IEquatable<Vector3>
{
    public float X { get; }

    public float Y { get; }

    public float Z { get; }

    public static Vector3 Zero => new(0.0f, 0.0f, 0.0f);

    public static Vector3 One => new(1.0f, 1.0f, 1.0f);

    public static Vector3 UnitX => new(1.0f, 0.0f, 0.0f);

    public static Vector3 UnitY => new(0.0f, 1.0f, 0.0f);

    public static Vector3 UnitZ => new(0.0f, 0.0f, 1.0f);

    public float Length =>
        MathF.Sqrt(
            (X * X) +
            (Y * Y) +
            (Z * Z));

    public float LengthSquared =>
        (X * X) +
        (Y * Y) +
        (Z * Z);

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 Normalized()
    {
        float length = Length;

        if (length <= float.Epsilon)
        {
            return Zero;
        }

        return this / length;
    }

    public static float Distance(
        Vector3 a,
        Vector3 b)
    {
        return (a - b).Length;
    }

    public static float DistanceSquared(
        Vector3 a,
        Vector3 b)
    {
        return (a - b).LengthSquared;
    }

    public static float Dot(
        Vector3 a,
        Vector3 b)
    {
        return
            (a.X * b.X) +
            (a.Y * b.Y) +
            (a.Z * b.Z);
    }

    public static Vector3 Cross(
        Vector3 a,
        Vector3 b)
    {
        return new Vector3(
            (a.Y * b.Z) - (a.Z * b.Y),
            (a.Z * b.X) - (a.X * b.Z),
            (a.X * b.Y) - (a.Y * b.X));
    }

    public static Vector3 Lerp(
        Vector3 a,
        Vector3 b,
        float amount)
    {
        return a + ((b - a) * amount);
    }

    public static Vector3 operator +(
        Vector3 left,
        Vector3 right)
    {
        return new Vector3(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z);
    }

    public static Vector3 operator -(
        Vector3 left,
        Vector3 right)
    {
        return new Vector3(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z);
    }

    public static Vector3 operator -(
        Vector3 value)
    {
        return new Vector3(
            -value.X,
            -value.Y,
            -value.Z);
    }

    public static Vector3 operator *(
        Vector3 value,
        float scalar)
    {
        return new Vector3(
            value.X * scalar,
            value.Y * scalar,
            value.Z * scalar);
    }

    public static Vector3 operator *(
        float scalar,
        Vector3 value)
    {
        return value * scalar;
    }

    public static Vector3 operator /(
        Vector3 value,
        float scalar)
    {
        if (MathF.Abs(scalar) <= float.Epsilon)
        {
            throw new DivideByZeroException();
        }

        return new Vector3(
            value.X / scalar,
            value.Y / scalar,
            value.Z / scalar);
    }

    public static bool operator ==(
        Vector3 left,
        Vector3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Vector3 left,
        Vector3 right)
    {
        return !left.Equals(right);
    }

    public bool Equals(Vector3 other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y) &&
               Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector3 other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}