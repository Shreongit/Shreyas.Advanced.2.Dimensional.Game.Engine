using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Sprites;

public sealed class SpriteBatch
{
    private readonly List<SpriteDrawData> _sprites = new();

    private bool _begun;

    public int Count => _sprites.Count;

    public bool IsBegun => _begun;

    public void Begin()
    {
        if (_begun)
        {
            throw new InvalidOperationException(
                "SpriteBatch has already begun.");
        }

        _sprites.Clear();
        _begun = true;
    }

    public void Draw(
        Sprite sprite)
    {
        ArgumentNullException.ThrowIfNull(sprite);
        EnsureBegun();

        if (sprite.Texture is null)
        {
            throw new InvalidOperationException(
                "Sprite must have a texture before it can be drawn.");
        }

        _sprites.Add(
            new SpriteDrawData(
                sprite.Texture,
                sprite.Region,
                sprite.Position,
                sprite.Size,
                sprite.Origin,
                sprite.Rotation,
                sprite.Color,
                sprite.Layer,
                sprite.FlipX,
                sprite.FlipY));
    }

    public void Draw(
        Texture2D texture,
        Vector2 position,
        Vector2 size,
        Color color,
        float rotation = 0.0f,
        Vector2? origin = null,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(texture);
        EnsureBegun();

        _sprites.Add(
            new SpriteDrawData(
                texture,
                null,
                position,
                size,
                origin ?? new Vector2(0.5f, 0.5f),
                rotation,
                color,
                layer,
                false,
                false));
    }

    public void Draw(
        TextureRegion region,
        Vector2 position,
        Vector2 size,
        Color color,
        float rotation = 0.0f,
        Vector2? origin = null,
        int layer = 0)
    {
        EnsureBegun();

        _sprites.Add(
            new SpriteDrawData(
                region.Texture,
                region,
                position,
                size,
                origin ?? new Vector2(0.5f, 0.5f),
                rotation,
                color,
                layer,
                false,
                false));
    }

    public IReadOnlyList<SpriteDrawData> End()
    {
        EnsureBegun();

        _sprites.Sort(
            static (left, right) =>
                left.Layer.CompareTo(right.Layer));

        _begun = false;

        return _sprites.ToArray();
    }

    public void Clear()
    {
        _sprites.Clear();
        _begun = false;
    }

    private void EnsureBegun()
    {
        if (!_begun)
        {
            throw new InvalidOperationException(
                "SpriteBatch.Begin() must be called before drawing.");
        }
    }
}

public readonly struct SpriteDrawData
{
    public Texture2D Texture { get; }

    public TextureRegion? Region { get; }

    public Vector2 Position { get; }

    public Vector2 Size { get; }

    public Vector2 Origin { get; }

    public float Rotation { get; }

    public Color Color { get; }

    public int Layer { get; }

    public bool FlipX { get; }

    public bool FlipY { get; }

    public SpriteDrawData(
        Texture2D texture,
        TextureRegion? region,
        Vector2 position,
        Vector2 size,
        Vector2 origin,
        float rotation,
        Color color,
        int layer,
        bool flipX,
        bool flipY)
    {
        Texture = texture;
        Region = region;
        Position = position;
        Size = size;
        Origin = origin;
        Rotation = rotation;
        Color = color;
        Layer = layer;
        FlipX = flipX;
        FlipY = flipY;
    }
}