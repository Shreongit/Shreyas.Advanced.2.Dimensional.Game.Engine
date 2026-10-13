using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics;

public readonly struct Ray
{
    public Vector2 Origin { get; }

    public Vector2 Direction { get; }

    public Ray(
        Vector2 origin,
        Vector2 direction)
    {
        Origin = origin;

        Vector2 normalized =
            direction.Normalized();

        if (normalized == Vector2.Zero)
        {
            throw new ArgumentException(
                "Ray direction must not be zero.",
                nameof(direction));
        }

        Direction = normalized;
    }

    public Vector2 GetPoint(float distance)
    {
        return Origin + Direction * distance;
    }
}

public readonly struct RaycastHit
{
    public Collider Collider { get; }

    public Vector2 Point { get; }

    public Vector2 Normal { get; }

    public float Distance { get; }

    public RaycastHit(
        Collider collider,
        Vector2 point,
        Vector2 normal,
        float distance)
    {
        ArgumentNullException.ThrowIfNull(collider);

        if (distance < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(distance));
        }

        Collider = collider;
        Point = point;
        Normal = normal;
        Distance = distance;
    }

    public override string ToString()
    {
        return $"Hit: {Collider}, " +
               $"Point={Point}, " +
               $"Normal={Normal}, " +
               $"Distance={Distance}";
    }
}