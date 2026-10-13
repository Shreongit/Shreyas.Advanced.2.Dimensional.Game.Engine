namespace SA2DGE.Engine.Math;

public readonly struct Matrix4 : IEquatable<Matrix4>
{
    public float M11 { get; }
    public float M12 { get; }
    public float M13 { get; }
    public float M14 { get; }

    public float M21 { get; }
    public float M22 { get; }
    public float M23 { get; }
    public float M24 { get; }

    public float M31 { get; }
    public float M32 { get; }
    public float M33 { get; }
    public float M34 { get; }

    public float M41 { get; }
    public float M42 { get; }
    public float M43 { get; }
    public float M44 { get; }

    public static Matrix4 Identity => new(
        1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f);

    public Matrix4(
        float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34,
        float m41, float m42, float m43, float m44)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;

        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;

        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;

        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;
    }

    public static Matrix4 CreateTranslation(Vector3 position)
    {
        return new Matrix4(
            1.0f, 0.0f, 0.0f, position.X,
            0.0f, 1.0f, 0.0f, position.Y,
            0.0f, 0.0f, 1.0f, position.Z,
            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 CreateScale(Vector3 scale)
    {
        return new Matrix4(
            scale.X, 0.0f, 0.0f, 0.0f,
            0.0f, scale.Y, 0.0f, 0.0f,
            0.0f, 0.0f, scale.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 CreateRotationX(float radians)
    {
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);

        return new Matrix4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, cos, -sin, 0.0f,
            0.0f, sin, cos, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 CreateRotationY(float radians)
    {
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);

        return new Matrix4(
            cos, 0.0f, sin, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            -sin, 0.0f, cos, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 CreateRotationZ(float radians)
    {
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);

        return new Matrix4(
            cos, -sin, 0.0f, 0.0f,
            sin, cos, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 CreateOrthographic(
        float width,
        float height,
        float nearPlane,
        float farPlane)
    {
        if (width <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        if (nearPlane >= farPlane)
        {
            throw new ArgumentException(
                "Near plane must be less than far plane.");
        }

        return new Matrix4(
            2.0f / width, 0.0f, 0.0f, 0.0f,
            0.0f, 2.0f / height, 0.0f, 0.0f,
            0.0f, 0.0f, -2.0f / (farPlane - nearPlane),
            -(farPlane + nearPlane) / (farPlane - nearPlane),
            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 CreateOrthographicOffCenter(
        float left,
        float right,
        float bottom,
        float top,
        float nearPlane,
        float farPlane)
    {
        if (MathF.Abs(right - left) <= float.Epsilon)
        {
            throw new ArgumentException(
                "Left and right cannot be equal.");
        }

        if (MathF.Abs(top - bottom) <= float.Epsilon)
        {
            throw new ArgumentException(
                "Bottom and top cannot be equal.");
        }

        if (MathF.Abs(farPlane - nearPlane) <= float.Epsilon)
        {
            throw new ArgumentException(
                "Near and far planes cannot be equal.");
        }

        return new Matrix4(
            2.0f / (right - left), 0.0f, 0.0f,
            -(right + left) / (right - left),

            0.0f, 2.0f / (top - bottom), 0.0f,
            -(top + bottom) / (top - bottom),

            0.0f, 0.0f, -2.0f / (farPlane - nearPlane),
            -(farPlane + nearPlane) / (farPlane - nearPlane),

            0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Matrix4 operator *(Matrix4 left, Matrix4 right)
    {
        return new Matrix4(
            left.M11 * right.M11 + left.M12 * right.M21 +
            left.M13 * right.M31 + left.M14 * right.M41,

            left.M11 * right.M12 + left.M12 * right.M22 +
            left.M13 * right.M32 + left.M14 * right.M42,

            left.M11 * right.M13 + left.M12 * right.M23 +
            left.M13 * right.M33 + left.M14 * right.M43,

            left.M11 * right.M14 + left.M12 * right.M24 +
            left.M13 * right.M34 + left.M14 * right.M44,

            left.M21 * right.M11 + left.M22 * right.M21 +
            left.M23 * right.M31 + left.M24 * right.M41,

            left.M21 * right.M12 + left.M22 * right.M22 +
            left.M23 * right.M32 + left.M24 * right.M42,

            left.M21 * right.M13 + left.M22 * right.M23 +
            left.M23 * right.M33 + left.M24 * right.M43,

            left.M21 * right.M14 + left.M22 * right.M24 +
            left.M23 * right.M34 + left.M24 * right.M44,

            left.M31 * right.M11 + left.M32 * right.M21 +
            left.M33 * right.M31 + left.M34 * right.M41,

            left.M31 * right.M12 + left.M32 * right.M22 +
            left.M33 * right.M32 + left.M34 * right.M42,

            left.M31 * right.M13 + left.M32 * right.M23 +
            left.M33 * right.M33 + left.M34 * right.M43,

            left.M31 * right.M14 + left.M32 * right.M24 +
            left.M33 * right.M34 + left.M34 * right.M44,

            left.M41 * right.M11 + left.M42 * right.M21 +
            left.M43 * right.M31 + left.M44 * right.M41,

            left.M41 * right.M12 + left.M42 * right.M22 +
            left.M43 * right.M32 + left.M44 * right.M42,

            left.M41 * right.M13 + left.M42 * right.M23 +
            left.M43 * right.M33 + left.M44 * right.M43,

            left.M41 * right.M14 + left.M42 * right.M24 +
            left.M43 * right.M34 + left.M44 * right.M44);
    }

    public static Vector3 Transform(
        Vector3 vector,
        Matrix4 matrix)
    {
        float x =
            matrix.M11 * vector.X +
            matrix.M12 * vector.Y +
            matrix.M13 * vector.Z +
            matrix.M14;

        float y =
            matrix.M21 * vector.X +
            matrix.M22 * vector.Y +
            matrix.M23 * vector.Z +
            matrix.M24;

        float z =
            matrix.M31 * vector.X +
            matrix.M32 * vector.Y +
            matrix.M33 * vector.Z +
            matrix.M34;

        return new Vector3(x, y, z);
    }

    public bool Equals(Matrix4 other)
    {
        return
            M11.Equals(other.M11) &&
            M12.Equals(other.M12) &&
            M13.Equals(other.M13) &&
            M14.Equals(other.M14) &&
            M21.Equals(other.M21) &&
            M22.Equals(other.M22) &&
            M23.Equals(other.M23) &&
            M24.Equals(other.M24) &&
            M31.Equals(other.M31) &&
            M32.Equals(other.M32) &&
            M33.Equals(other.M33) &&
            M34.Equals(other.M34) &&
            M41.Equals(other.M41) &&
            M42.Equals(other.M42) &&
            M43.Equals(other.M43) &&
            M44.Equals(other.M44);
    }

    public override bool Equals(object? obj)
    {
        return obj is Matrix4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            HashCode.Combine(M11, M12, M13, M14),
            HashCode.Combine(M21, M22, M23, M24),
            HashCode.Combine(M31, M32, M33, M34),
            HashCode.Combine(M41, M42, M43, M44));
    }

    public static bool operator ==(Matrix4 left, Matrix4 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Matrix4 left, Matrix4 right)
    {
        return !left.Equals(right);
    }
}