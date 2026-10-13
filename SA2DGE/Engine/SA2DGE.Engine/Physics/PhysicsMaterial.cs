namespace SA2DGE.Engine.Physics;

public sealed class PhysicsMaterial
{
    public float Friction { get; set; }

    public float Restitution { get; set; }

    public float Density { get; set; }

    public PhysicsMaterial(
        float friction = 0.5f,
        float restitution = 0.0f,
        float density = 1.0f)
    {
        if (friction < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(friction));
        }

        if (restitution < 0.0f ||
            restitution > 1.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(restitution));
        }

        if (density <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(density));
        }

        Friction = friction;
        Restitution = restitution;
        Density = density;
    }

    public static PhysicsMaterial Default =>
        new();

    public static PhysicsMaterial Bouncy =>
        new(
            friction: 0.3f,
            restitution: 1.0f,
            density: 1.0f);

    public static PhysicsMaterial Ice =>
        new(
            friction: 0.05f,
            restitution: 0.0f,
            density: 1.0f);
}