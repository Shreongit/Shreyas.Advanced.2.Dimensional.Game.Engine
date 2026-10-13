using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics.Shapes;

public readonly struct CircleShape : IEquatable<CircleShape>
{
    public float Radius { get; }

    public float Diameter =>
        Radius * 2.0f;

    public float Area =>
        MathF.PI * Radius * Radius;

    public CircleShape(float radius)
    {
        if (radius <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(radius),
                "Circle radius must be greater than zero.");
        }

        Radius = radius;
    }

    public Circle GetCircle(
        Vector2 position)
    {
        return new Circle(
            position,
            Radius);
    }

    public Bounds GetBounds(
        Vector2 position)
    {
        Vector2 radius =
            new(Radius, Radius);

        return new Bounds(
            position - radius,
            position + radius);
    }

    public bool Contains(
        Vector2 position,
        Vector2 point)
    {
        return Vector2.DistanceSquared(
                   position,
                   point) <=
               Radius * Radius;
    }

    public bool Intersects(
        Vector2 position,
        CircleShape other,
        Vector2 otherPosition)
    {
        float combinedRadius =
            Radius + other.Radius;

        return Vector2.DistanceSquared(
                   position,
                   otherPosition) <=
               combinedRadius * combinedRadius;
    }

    public bool Intersects(
        Vector2 position,
        BoxShape box,
        Vector2 boxPosition)
    {
        return box
            .GetRectangle(boxPosition)
            .Contains(position)
            ||
            GetCircle(position)
                .Intersects(
                    box.GetRectangle(boxPosition));
    }

    public bool Equals(CircleShape other)
    {
        return Radius.Equals(other.Radius);
    }

    public override bool Equals(object? obj)
    {
        return obj is CircleShape other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return Radius.GetHashCode();
    }

    public static bool operator ==(
        CircleShape left,
        CircleShape right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        CircleShape left,
        CircleShape right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Circle Radius={Radius}";
    }
}