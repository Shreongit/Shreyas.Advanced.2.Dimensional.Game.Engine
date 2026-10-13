namespace SA2DGE.Engine.Math;

public readonly struct Bounds : IEquatable<Bounds>
{
    public Vector2 Min { get; }

    public Vector2 Max { get; }

    public Vector2 Center =>
        (Min + Max) * 0.5f;

    public Vector2 Size =>
        Max - Min;

    public float Width =>
        Max.X - Min.X;

    public float Height =>
        Max.Y - Min.Y;

    public Bounds(
        Vector2 min,
        Vector2 max)
    {
        Min = new Vector2(
            MathF.Min(min.X, max.X),
            MathF.Min(min.Y, max.Y));

        Max = new Vector2(
            MathF.Max(min.X, max.X),
            MathF.Max(min.Y, max.Y));
    }

    public static Bounds FromRectangle(
        Rectangle rectangle)
    {
        return new Bounds(
            new Vector2(
                rectangle.Left,
                rectangle.Top),
            new Vector2(
                rectangle.Right,
                rectangle.Bottom));
    }

    public static Bounds FromPoints(
        IEnumerable<Vector2> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        using IEnumerator<Vector2> enumerator =
            points.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return new Bounds(
                Vector2.Zero,
                Vector2.Zero);
        }

        Vector2 min = enumerator.Current;
        Vector2 max = enumerator.Current;

        while (enumerator.MoveNext())
        {
            Vector2 point = enumerator.Current;

            min = new Vector2(
                MathF.Min(min.X, point.X),
                MathF.Min(min.Y, point.Y));

            max = new Vector2(
                MathF.Max(max.X, point.X),
                MathF.Max(max.Y, point.Y));
        }

        return new Bounds(min, max);
    }

    public bool Contains(Vector2 point)
    {
        return point.X >= Min.X &&
               point.X <= Max.X &&
               point.Y >= Min.Y &&
               point.Y <= Max.Y;
    }

    public bool Contains(Bounds bounds)
    {
        return bounds.Min.X >= Min.X &&
               bounds.Max.X <= Max.X &&
               bounds.Min.Y >= Min.Y &&
               bounds.Max.Y <= Max.Y;
    }

    public bool Intersects(Bounds bounds)
    {
        return Min.X <= bounds.Max.X &&
               Max.X >= bounds.Min.X &&
               Min.Y <= bounds.Max.Y &&
               Max.Y >= bounds.Min.Y;
    }

    public Bounds Encapsulate(Vector2 point)
    {
        return new Bounds(
            new Vector2(
                MathF.Min(Min.X, point.X),
                MathF.Min(Min.Y, point.Y)),
            new Vector2(
                MathF.Max(Max.X, point.X),
                MathF.Max(Max.Y, point.Y)));
    }

    public Bounds Encapsulate(Bounds bounds)
    {
        return new Bounds(
            new Vector2(
                MathF.Min(Min.X, bounds.Min.X),
                MathF.Min(Min.Y, bounds.Min.Y)),
            new Vector2(
                MathF.Max(Max.X, bounds.Max.X),
                MathF.Max(Max.Y, bounds.Max.Y)));
    }

    public Rectangle ToRectangle()
    {
        return new Rectangle(
            Min.X,
            Min.Y,
            Width,
            Height);
    }

    public bool Equals(Bounds other)
    {
        return Min.Equals(other.Min) &&
               Max.Equals(other.Max);
    }

    public override bool Equals(object? obj)
    {
        return obj is Bounds other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    public static bool operator ==(
        Bounds left,
        Bounds right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Bounds left,
        Bounds right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Min: {Min}, Max: {Max}";
    }
}