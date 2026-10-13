namespace SA2DGE.Engine.Math;

public readonly struct Vector2 : IEquatable<Vector2>
{
    public float X { get; }

    public float Y { get; }

    public static Vector2 Zero => new(0.0f, 0.0f);

    public static Vector2 One => new(1.0f, 1.0f);

    public static Vector2 UnitX => new(1.0f, 0.0f);

    public static Vector2 UnitY => new(0.0f, 1.0f);

    public float Length =>
        MathF.Sqrt((X * X) + (Y * Y));

    public float LengthSquared =>
        (X * X) + (Y * Y);

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vector2 Normalized()
    {
        float length = Length;

        if (length <= float.Epsilon)
        {
            return Zero;
        }

        return this / length;
    }

    public static float Distance(
        Vector2 a,
        Vector2 b)
    {
        return (a - b).Length;
    }

    public static float DistanceSquared(
        Vector2 a,
        Vector2 b)
    {
        return (a - b).LengthSquared;
    }

    public static float Dot(
        Vector2 a,
        Vector2 b)
    {
        return (a.X * b.X) + (a.Y * b.Y);
    }

    public static Vector2 Lerp(
        Vector2 a,
        Vector2 b,
        float amount)
    {
        return a + ((b - a) * amount);
    }

    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
        return new Vector2(
            left.X + right.X,
            left.Y + right.Y);
    }

    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        return new Vector2(
            left.X - right.X,
            left.Y - right.Y);
    }

    public static Vector2 operator -(Vector2 value)
    {
        return new Vector2(-value.X, -value.Y);
    }

    public static Vector2 operator *(Vector2 value, float scalar)
    {
        return new Vector2(
            value.X * scalar,
            value.Y * scalar);
    }

    public static Vector2 operator *(float scalar, Vector2 value)
    {
        return value * scalar;
    }

    public static Vector2 operator /(Vector2 value, float scalar)
    {
        if (MathF.Abs(scalar) <= float.Epsilon)
        {
            throw new DivideByZeroException();
        }

        return new Vector2(
            value.X / scalar,
            value.Y / scalar);
    }

    public static bool operator ==(
        Vector2 left,
        Vector2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Vector2 left,
        Vector2 right)
    {
        return !left.Equals(right);
    }

    public bool Equals(Vector2 other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2 other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}