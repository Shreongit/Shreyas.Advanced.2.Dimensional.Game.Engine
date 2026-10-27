
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics;

/// <summary>
/// Owns and renders a full-viewport background texture.
/// The supplied texture is owned by this component and disposed with it.
/// </summary>
public sealed class BackgroundLayer : IDisposable
{
    private readonly Texture2D _texture;
    private readonly SpriteSheet _spriteSheet;
    private bool _disposed;

    public int Layer { get; }

    public BackgroundLayer(
        Texture2D texture,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(texture);

        _texture = texture;
        _spriteSheet = new SpriteSheet(
            columns: 1,
            rows: 1);

        Layer = layer;
    }

    public void Draw(
        Renderer2D renderer,
        float viewportWidth,
        float viewportHeight)
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);

        ArgumentNullException.ThrowIfNull(renderer);

        if (!float.IsFinite(viewportWidth) ||
            viewportWidth <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(viewportWidth),
                "Viewport width must be finite and positive.");
        }

        if (!float.IsFinite(viewportHeight) ||
            viewportHeight <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(viewportHeight),
                "Viewport height must be finite and positive.");
        }

        renderer.DrawSpriteFrame(
            position: new Vector2(
                viewportWidth / 2.0f,
                viewportHeight / 2.0f),
            size: new Vector2(
                viewportWidth,
                viewportHeight),
            color: new Color(
                1.0f, 1.0f, 1.0f, 1.0f),
            texture: _texture,
            spriteSheet: _spriteSheet,
            frameIndex: 0,
            layer: Layer);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _texture.Dispose();
        _disposed = true;
    }
}
