namespace SA2DGE.Engine.Math;

public readonly struct Rectangle : IEquatable<Rectangle>
{
    public float X { get; }

    public float Y { get; }

    public float Width { get; }

    public float Height { get; }

    public float Left => X;

    public float Right => X + Width;

    public float Top => Y;

    public float Bottom => Y + Height;

    public Vector2 Position => new(X, Y);

    public Vector2 Size => new(Width, Height);

    public Vector2 Center =>
        new(
            X + Width * 0.5f,
            Y + Height * 0.5f);

    public Rectangle(
        float x,
        float y,
        float width,
        float height)
    {
        if (width < 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height < 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Contains(Vector2 point)
    {
        return point.X >= Left &&
               point.X <= Right &&
               point.Y >= Top &&
               point.Y <= Bottom;
    }

    public bool Contains(Rectangle rectangle)
    {
        return rectangle.Left >= Left &&
               rectangle.Right <= Right &&
               rectangle.Top >= Top &&
               rectangle.Bottom <= Bottom;
    }

    public bool Intersects(Rectangle rectangle)
    {
        return Left < rectangle.Right &&
               Right > rectangle.Left &&
               Top < rectangle.Bottom &&
               Bottom > rectangle.Top;
    }

    public Rectangle Intersection(Rectangle rectangle)
    {
        float left = MathF.Max(Left, rectangle.Left);
        float top = MathF.Max(Top, rectangle.Top);
        float right = MathF.Min(Right, rectangle.Right);
        float bottom = MathF.Min(Bottom, rectangle.Bottom);

        if (right <= left || bottom <= top)
        {
            return new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);
        }

        return new Rectangle(
            left,
            top,
            right - left,
            bottom - top);
    }

    public Rectangle Union(Rectangle rectangle)
    {
        float left = MathF.Min(Left, rectangle.Left);
        float top = MathF.Min(Top, rectangle.Top);
        float right = MathF.Max(Right, rectangle.Right);
        float bottom = MathF.Max(Bottom, rectangle.Bottom);

        return new Rectangle(
            left,
            top,
            right - left,
            bottom - top);
    }

    public Rectangle Offset(Vector2 amount)
    {
        return new Rectangle(
            X + amount.X,
            Y + amount.Y,
            Width,
            Height);
    }

    public Rectangle Inflate(
        float horizontal,
        float vertical)
    {
        return new Rectangle(
            X - horizontal,
            Y - vertical,
            Width + horizontal * 2.0f,
            Height + vertical * 2.0f);
    }

    public bool Equals(Rectangle other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y) &&
               Width.Equals(other.Width) &&
               Height.Equals(other.Height);
    }

    public override bool Equals(object? obj)
    {
        return obj is Rectangle other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            X,
            Y,
            Width,
            Height);
    }

    public static bool operator ==(
        Rectangle left,
        Rectangle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Rectangle left,
        Rectangle right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Width}, {Height})";
    }
}