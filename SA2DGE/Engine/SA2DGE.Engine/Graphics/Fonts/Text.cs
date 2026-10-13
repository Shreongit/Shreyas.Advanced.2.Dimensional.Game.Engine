using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Fonts;

public sealed class Text
{
    public string Content { get; set; }

    public Font? Font { get; set; }

    public Vector2 Position { get; set; }

    public float Size { get; set; }

    public Color Color { get; set; }

    public float Rotation { get; set; }

    public Vector2 Origin { get; set; }

    public int Layer { get; set; }

    public Text(
        string content,
        Font? font = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        Content = content;
        Font = font;
        Position = Vector2.Zero;
        Size = font?.Size ?? 16.0f;
        Color = Color.White;
        Rotation = 0.0f;
        Origin = Vector2.Zero;
        Layer = 0;
    }

    public Vector2 Measure()
    {
        if (Font is null)
        {
            return Vector2.Zero;
        }

        float width =
            Font.MeasureWidth(Content);

        float height =
            Font.MeasureHeight(Content);

        return new Vector2(
            width,
            height);
    }

    public Rectangle GetBounds()
    {
        Vector2 size = Measure();

        return new Rectangle(
            Position.X - size.X * Origin.X,
            Position.Y - size.Y * Origin.Y,
            size.X,
            size.Y);
    }

    public void SetFont(Font font)
    {
        ArgumentNullException.ThrowIfNull(font);

        Font = font;

        if (Size <= 0.0f)
        {
            Size = font.Size;
        }
    }

    public override string ToString()
    {
        return Content;
    }
}