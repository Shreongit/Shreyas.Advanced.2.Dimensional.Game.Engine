using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;

/// <summary>
/// Represents a rectangular environment object that can be
/// rendered and used for player collision.
/// </summary>
internal sealed class EnvironmentObject
{
    public Vector2 Position { get; }

    public Vector2 Size { get; }

    public Color Color { get; }

    public int Layer { get; }

    /// <summary>
    /// Collision bounds expressed as a rectangle whose origin
    /// is its top-left corner.
    /// </summary>
    public Rectangle CollisionBounds =>
        new(
            Position.X - Size.X / 2.0f,
            Position.Y - Size.Y / 2.0f,
            Size.X,
            Size.Y);

    public EnvironmentObject(
        Vector2 position,
        Vector2 size,
        Color color,
        int layer = 1)
    {
        if (!float.IsFinite(position.X) ||
            !float.IsFinite(position.Y))
        {
            throw new ArgumentOutOfRangeException(
                nameof(position),
                "Position must contain finite coordinates.");
        }

        if (!float.IsFinite(size.X) ||
            !float.IsFinite(size.Y) ||
            size.X <= 0.0f ||
            size.Y <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "Object dimensions must be finite and positive.");
        }

        Position = position;
        Size = size;
        Color = color;
        Layer = layer;
    }

    public void Draw(Renderer2D renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        renderer.DrawRectangle(
            position: Position,
            size: Size,
            color: Color,
            layer: Layer);
    }
}