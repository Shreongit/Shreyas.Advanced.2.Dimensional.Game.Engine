namespace SA2DGE.Engine.Math;

public readonly struct Circle : IEquatable<Circle>
{
    public Vector2 Center { get; }

    public float Radius { get; }

    public float Diameter => Radius * 2.0f;

    public float Circumference =>
        2.0f * MathF.PI * Radius;

    public float Area =>
        MathF.PI * Radius * Radius;

    public Circle(
        Vector2 center,
        float radius)
    {
        if (radius < 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(radius));
        }

        Center = center;
        Radius = radius;
    }

    public bool Contains(Vector2 point)
    {
        return Vector2.DistanceSquared(
                   Center,
                   point) <=
               Radius * Radius;
    }

    public bool Intersects(Circle other)
    {
        float combinedRadius =
            Radius + other.Radius;

        return Vector2.DistanceSquared(
                   Center,
                   other.Center) <=
               combinedRadius * combinedRadius;
    }

    public bool Intersects(Rectangle rectangle)
    {
        float closestX = MathF.Max(
            rectangle.Left,
            MathF.Min(Center.X, rectangle.Right));

        float closestY = MathF.Max(
            rectangle.Top,
            MathF.Min(Center.Y, rectangle.Bottom));

        float distanceX = Center.X - closestX;
        float distanceY = Center.Y - closestY;

        return (distanceX * distanceX) +
               (distanceY * distanceY) <=
               Radius * Radius;
    }

    public Circle Offset(Vector2 amount)
    {
        return new Circle(
            Center + amount,
            Radius);
    }

    public bool Equals(Circle other)
    {
        return Center.Equals(other.Center) &&
               Radius.Equals(other.Radius);
    }

    public override bool Equals(object? obj)
    {
        return obj is Circle other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Center,
            Radius);
    }

    public static bool operator ==(
        Circle left,
        Circle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        Circle left,
        Circle right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Center: {Center}, Radius: {Radius}";
    }
}