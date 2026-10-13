using SA2DGE.Engine.Math;
using SA2DGE.Engine.Physics.Shapes;

namespace SA2DGE.Engine.Physics;

public sealed class Collider
{
    private readonly object _shape;

    public object Shape => _shape;

    public Vector2 Offset { get; set; }

    public PhysicsMaterial Material { get; set; }

    public Rigidbody? Rigidbody { get; }

    public bool IsTrigger { get; set; }

    public Collider(
        BoxShape shape,
        Rigidbody? rigidbody = null,
        Vector2? offset = null)
    {
        _shape = shape;
        Rigidbody = rigidbody;
        Offset = offset ?? Vector2.Zero;
        Material = rigidbody?.Material ?? PhysicsMaterial.Default;
    }

    public Collider(
        CircleShape shape,
        Rigidbody? rigidbody = null,
        Vector2? offset = null)
    {
        _shape = shape;
        Rigidbody = rigidbody;
        Offset = offset ?? Vector2.Zero;
        Material = rigidbody?.Material ?? PhysicsMaterial.Default;
    }

    public Collider(
        PolygonShape shape,
        Rigidbody? rigidbody = null,
        Vector2? offset = null)
    {
        ArgumentNullException.ThrowIfNull(shape);

        _shape = shape;
        Rigidbody = rigidbody;
        Offset = offset ?? Vector2.Zero;
        Material = rigidbody?.Material ?? PhysicsMaterial.Default;
    }

    public Vector2 Position =>
        (Rigidbody?.Position ?? Vector2.Zero) +
        Offset;

    public float Rotation =>
        Rigidbody?.Rotation ?? 0.0f;

    public Bounds GetBounds()
    {
        Vector2 position = Position;

        return _shape switch
        {
            BoxShape box =>
                box.GetBounds(position),

            CircleShape circle =>
                circle.GetBounds(position),

            PolygonShape polygon =>
                polygon.GetBounds(position),

            _ => throw new InvalidOperationException(
                $"Unsupported collider shape: {_shape.GetType().Name}.")
        };
    }

    public bool Contains(Vector2 point)
    {
        Vector2 position = Position;

        return _shape switch
        {
            BoxShape box =>
                box.Contains(position, point),

            CircleShape circle =>
                circle.Contains(position, point),

            PolygonShape polygon =>
                polygon.Contains(position, point),

            _ => false
        };
    }

    public void SetMaterial(
        PhysicsMaterial material)
    {
        ArgumentNullException.ThrowIfNull(material);

        Material = material;

        if (Rigidbody is not null)
        {
            Rigidbody.Material = material;
        }
    }

    public override string ToString()
    {
        return $"Collider: {_shape.GetType().Name}";
    }
}