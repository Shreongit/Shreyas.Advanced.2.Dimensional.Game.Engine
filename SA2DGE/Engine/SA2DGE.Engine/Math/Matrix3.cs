namespace SA2DGE.Engine.Math;

public readonly struct Matrix3 : IEquatable<Matrix3>
{
    public float M11 { get; }
    public float M12 { get; }
    public float M13 { get; }

    public float M21 { get; }
    public float M22 { get; }
    public float M23 { get; }

    public float M31 { get; }
    public float M32 { get; }
    public float M33 { get; }

    public static Matrix3 Identity => new(
        1.0f, 0.0f, 0.0f,
        0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 1.0f);

    public Matrix3(
        float m11, float m12, float m13,
        float m21, float m22, float m23,
        float m31, float m32, float m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;

        M21 = m21;
        M22 = m22;
        M23 = m23;

        M31 = m31;
        M32 = m32;
        M33 = m33;
    }

    public static Matrix3 CreateTranslation(Vector2 position)
    {
        return new Matrix3(
            1.0f, 0.0f, position.X,
            0.0f, 1.0f, position.Y,
            0.0f, 0.0f, 1.0f);
    }

    public static Matrix3 CreateScale(Vector2 scale)
    {
        return new Matrix3(
            scale.X, 0.0f, 0.0f,
            0.0f, scale.Y, 0.0f,
            0.0f, 0.0f, 1.0f);
    }

    public static Matrix3 CreateRotation(float radians)
    {
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);

        return new Matrix3(
            cos, -sin, 0.0f,
            sin, cos, 0.0f,
            0.0f, 0.0f, 1.0f);
    }

    public static Matrix3 operator *(Matrix3 left, Matrix3 right)
    {
        return new Matrix3(
            left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
            left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
            left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,

            left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
            left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
            left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,

            left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
            left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
            left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33);
    }

    public static Vector2 Transform(
        Vector2 vector,
        Matrix3 matrix)
    {
        float x =
            (matrix.M11 * vector.X) +
            (matrix.M12 * vector.Y) +
            matrix.M13;

        float y =
            (matrix.M21 * vector.X) +
            (matrix.M22 * vector.Y) +
            matrix.M23;

        return new Vector2(x, y);
    }

    public bool Equals(Matrix3 other)
    {
        return M11.Equals(other.M11) &&
               M12.Equals(other.M12) &&
               M13.Equals(other.M13) &&
               M21.Equals(other.M21) &&
               M22.Equals(other.M22) &&
               M23.Equals(other.M23) &&
               M31.Equals(other.M31) &&
               M32.Equals(other.M32) &&
               M33.Equals(other.M33);
    }

    public override bool Equals(object? obj)
    {
        return obj is Matrix3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        int first = HashCode.Combine(
            M11,
            M12,
            M13);

        int second = HashCode.Combine(
            M21,
            M22,
            M23);

        int third = HashCode.Combine(
            M31,
            M32,
            M33);

        return HashCode.Combine(
            first,
            second,
            third);
    }   

    public static bool operator ==(Matrix3 left, Matrix3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Matrix3 left, Matrix3 right)
    {
        return !left.Equals(right);
    }
}