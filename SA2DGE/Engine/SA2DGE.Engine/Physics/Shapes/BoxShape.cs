using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics.Shapes;

public readonly struct BoxShape : IEquatable<BoxShape>
{
    public Vector2 Size { get; }

    public Vector2 HalfSize =>
        Size * 0.5f;

    public float Width =>
        Size.X;

    public float Height =>
        Size.Y;

    public float Area =>
        Size.X * Size.Y;

    public BoxShape(Vector2 size)
    {
        if (size.X <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "Box width must be greater than zero.");
        }

        if (size.Y <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "Box height must be greater than zero.");
        }

        Size = size;
    }

    public BoxShape(
        float width,
        float height)
        : this(new Vector2(width, height))
    {
    }

    public Bounds GetBounds(
        Vector2 position)
    {
        Vector2 halfSize = HalfSize;

        return new Bounds(
            position - halfSize,
            position + halfSize);
    }

    public Rectangle GetRectangle(
        Vector2 position)
    {
        Vector2 halfSize = HalfSize;

        return new Rectangle(
            position.X - halfSize.X,
            position.Y - halfSize.Y,
            Size.X,
            Size.Y);
    }

    public bool Contains(
        Vector2 position,
        Vector2 point)
    {
        return GetBounds(position)
            .Contains(point);
    }

    public bool Intersects(
        Vector2 position,
        BoxShape other,
        Vector2 otherPosition)
    {
        return GetBounds(position)
            .Intersects(
                other.GetBounds(otherPosition));
    }

    public bool Equals(BoxShape other)
    {
        return Size == other.Size;
    }

    public override bool Equals(object? obj)
    {
        return obj is BoxShape other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return Size.GetHashCode();
    }

    public static bool operator ==(
        BoxShape left,
        BoxShape right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        BoxShape left,
        BoxShape right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Box {Size}";
    }
}