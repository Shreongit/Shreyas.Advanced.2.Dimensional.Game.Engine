using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics;

public readonly struct RenderCommand
{
    public RenderCommandType Type { get; }

    public Vector2 Position { get; }

    public Vector2 Size { get; }

    public float Rotation { get; }

    public Color Color { get; }

    public int Layer { get; }

    public object? Resource { get; }

    public RenderCommand(
        RenderCommandType type,
        Vector2 position,
        Vector2 size,
        float rotation,
        Color color,
        int layer = 0,
        object? resource = null)
    {
        Type = type;
        Position = position;
        Size = size;
        Rotation = rotation;
        Color = color;
        Layer = layer;
        Resource = resource;
    }

    public static RenderCommand Sprite(
        Vector2 position,
        Vector2 size,
        Color color,
        float rotation = 0.0f,
        int layer = 0,
        object? texture = null)
    {
        return new RenderCommand(
            RenderCommandType.Sprite,
            position,
            size,
            rotation,
            color,
            layer,
            texture);
    }

    
    public static RenderCommand Rectangle(
        Vector2 position,
        Vector2 size,
        Color color,
        int layer = 0,
        float rotation = 0.0f)
    {
        return new RenderCommand(
            RenderCommandType.Rectangle,
            position,
            size,
            rotation,
            color,
            layer);
    }


    public static RenderCommand Circle(
        Vector2 position,
        float radius,
        Color color,
        int layer = 0)
    {
        return new RenderCommand(
            RenderCommandType.Circle,
            position,
            new Vector2(radius, radius),
            0.0f,
            color,
            layer);
    }

    public static RenderCommand Line(
        Vector2 start,
        Vector2 end,
        Color color,
        int layer = 0)
    {
        return new RenderCommand(
            RenderCommandType.Line,
            start,
            end,
            0.0f,
            color,
            layer);
    }

    public static RenderCommand Text(
        Vector2 position,
        Color color,
        object text,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(text);

        return new RenderCommand(
            RenderCommandType.Text,
            position,
            Vector2.Zero,
            0.0f,
            color,
            layer,
            text);
    }

    public override string ToString()
    {
        return $"{Type} @ {Position}, Layer={Layer}";
    }
}

public enum RenderCommandType
{
    Sprite,
    Rectangle,
    Circle,
    Line,
    Text
}