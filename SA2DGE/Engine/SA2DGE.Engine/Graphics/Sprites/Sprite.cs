using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Sprites;

public sealed class Sprite
{
    public Texture2D? Texture { get; private set; }

    public TextureRegion? Region { get; private set; }

    public Vector2 Position { get; set; }

    public Vector2 Size { get; set; }

    public Vector2 Origin { get; set; }

    public float Rotation { get; set; }

    public Color Color { get; set; }

    public int Layer { get; set; }

    public bool FlipX { get; set; }

    public bool FlipY { get; set; }

    public Sprite()
    {
        Position = Vector2.Zero;
        Size = Vector2.One;
        Origin = new Vector2(0.5f, 0.5f);
        Rotation = 0.0f;
        Color = Color.White;
        Layer = 0;
        FlipX = false;
        FlipY = false;
    }

    public Sprite(
        Texture2D texture,
        Vector2? size = null)
        : this()
    {
        SetTexture(texture);

        Size = size ?? new Vector2(
            texture.Width,
            texture.Height);
    }

    public Sprite(
        TextureRegion region,
        Vector2? size = null)
        : this()
    {
        SetRegion(region);

        Size = size ?? new Vector2(
            region.Width,
            region.Height);
    }

    public void SetTexture(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);

        Texture = texture;
        Region = null;
    }

    public void SetRegion(TextureRegion region)
    {
        Texture = region.Texture;
        Region = region;
    }

    public void Reset()
    {
        Texture = null;
        Region = null;

        Position = Vector2.Zero;
        Size = Vector2.One;
        Origin = new Vector2(0.5f, 0.5f);
        Rotation = 0.0f;
        Color = Color.White;
        Layer = 0;
        FlipX = false;
        FlipY = false;
    }

    public Vector2 GetOriginPosition()
    {
        return new Vector2(
            Size.X * Origin.X,
            Size.Y * Origin.Y);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            Position.X - Size.X * Origin.X,
            Position.Y - Size.Y * Origin.Y,
            Size.X,
            Size.Y);
    }
}