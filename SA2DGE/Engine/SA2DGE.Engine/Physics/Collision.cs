using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics;

public readonly struct Collision
{
    public Collider ColliderA { get; }

    public Collider ColliderB { get; }

    public Vector2 Normal { get; }

    public Vector2 ContactPoint { get; }

    public float Penetration { get; }

    public bool IsTrigger =>
        ColliderA.IsTrigger ||
        ColliderB.IsTrigger;

    public Collision(
        Collider colliderA,
        Collider colliderB,
        Vector2 normal,
        Vector2 contactPoint,
        float penetration)
    {
        ArgumentNullException.ThrowIfNull(colliderA);
        ArgumentNullException.ThrowIfNull(colliderB);

        if (penetration < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(penetration));
        }

        ColliderA = colliderA;
        ColliderB = colliderB;
        Normal = normal;
        ContactPoint = contactPoint;
        Penetration = penetration;
    }

    public Vector2 RelativeVelocity
    {
        get
        {
            Vector2 velocityA =
                ColliderA.Rigidbody?.Velocity ??
                Vector2.Zero;

            Vector2 velocityB =
                ColliderB.Rigidbody?.Velocity ??
                Vector2.Zero;

            return velocityB - velocityA;
        }
    }

    public float RelativeSpeed =>
        RelativeVelocity.Length;

    public override string ToString()
    {
        return $"Collision: {ColliderA} <-> {ColliderB}, " +
               $"Normal={Normal}, " +
               $"Penetration={Penetration}";
    }
}