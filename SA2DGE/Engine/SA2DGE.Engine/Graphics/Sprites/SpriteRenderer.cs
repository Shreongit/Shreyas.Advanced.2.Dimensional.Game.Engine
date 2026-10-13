using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Sprites;

public abstract class SpriteRenderer
{
    private SpriteBatch? _batch;

    public bool IsRendering => _batch?.IsBegun ?? false;

    public void Begin()
    {
        if (_batch is not null)
        {
            throw new InvalidOperationException(
                "Sprite renderer has already begun rendering.");
        }

        _batch = new SpriteBatch();
        _batch.Begin();
    }

    public void Draw(Sprite sprite)
    {
        ArgumentNullException.ThrowIfNull(sprite);
        EnsureRendering();

        _batch!.Draw(sprite);
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
        EnsureRendering();

        _batch!.Draw(
            texture,
            position,
            size,
            color,
            rotation,
            origin,
            layer);
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
        EnsureRendering();

        _batch!.Draw(
            region,
            position,
            size,
            color,
            rotation,
            origin,
            layer);
    }

    public void End()
    {
        EnsureRendering();

        IReadOnlyList<SpriteDrawData> sprites =
            _batch!.End();

        try
        {
            RenderSprites(sprites);
        }
        finally
        {
            _batch = null;
        }
    }

    protected abstract void RenderSprites(
        IReadOnlyList<SpriteDrawData> sprites);

    private void EnsureRendering()
    {
        if (_batch is null ||
            !_batch.IsBegun)
        {
            throw new InvalidOperationException(
                "Begin() must be called before rendering sprites.");
        }
    }
}