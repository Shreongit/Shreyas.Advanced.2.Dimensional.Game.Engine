using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Graphics.Cameras;

public abstract class Renderer2D : Renderer
{
    private readonly RenderQueue _renderQueue = new();
    
    /// <summary>
    /// Gets or sets the active 2D camera used by the renderer.
    /// Null preserves the existing screen-space rendering behavior.
    /// </summary>
    public Camera2D? Camera { get; set; }

    public override string Name => "SA2DGE Renderer2D";

    public RenderQueue RenderQueue =>
        _renderQueue;

    public void Draw(RenderCommand command)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        _renderQueue.Submit(command);
    }

    public void DrawSprite(
        Vector2 position,
        Vector2 size,
        Color color,
        float rotation = 0.0f,
        int layer = 0,
        object? texture = null,
        bool flipHorizontal = false,
        bool flipVertical = false,
        Vector2? uvOffset = null,
        Vector2? uvSize = null,
        CoordinateSpace space = CoordinateSpace.Screen)
    {
        Draw(
            RenderCommand.Sprite(
                position,
                size,
                color,
                rotation,
                layer,
                texture,
                flipHorizontal,
                flipVertical,
                uvOffset,
                uvSize,
                space));
                
    }
    
    
    public void DrawSpriteFrame(
        Vector2 position,
        Vector2 size,
        Color color,
        object? texture,
        SpriteSheet spriteSheet,
        int frameIndex,
        float rotation = 0.0f,
        int layer = 0,
        bool flipHorizontal = false,
        bool flipVertical = false,
        CoordinateSpace space = CoordinateSpace.Screen)
    {
        ArgumentNullException.ThrowIfNull(spriteSheet);

        Vector2 uvOffset =
            spriteSheet.GetFrameUVOffset(frameIndex);

        DrawSprite(
            position,
            size,
            color,
            rotation,
            layer,
            texture,
            flipHorizontal,
            flipVertical,
            uvOffset,
            spriteSheet.FrameUVSize,
            space);
    }


    
    public void DrawRectangle(
        Vector2 position,
        Vector2 size,
        Color color,
        int layer = 0,
        float rotation = 0.0f,
        CoordinateSpace space = CoordinateSpace.Screen)
    {
        Draw(
            RenderCommand.Rectangle(
                position,
                size,
                color,
                layer,
                rotation,
                space));
    }


    public void DrawCircle(
        Vector2 position,
        float radius,
        Color color,
        int layer = 0)
    {
        Draw(
            RenderCommand.Circle(
                position,
                radius,
                color,
                layer));
    }

    public void DrawLine(
        Vector2 start,
        Vector2 end,
        Color color,
        int layer = 0)
    {
        Draw(
            RenderCommand.Line(
                start,
                end,
                color,
                layer));
    }

    public void DrawText(
        Vector2 position,
        object text,
        Color color,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(text);

        Draw(
            RenderCommand.Text(
                position,
                color,
                text,
                layer));
    }

    public void Flush()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        _renderQueue.Sort();

        _renderQueue.Execute(
            ExecuteCommand);

        _renderQueue.Clear();
    }

    protected override void OnBeginFrame()
    {
        _renderQueue.Clear();
        BeginFrame2D();
    }

    protected override void OnEndFrame()
    {
        Flush();
        EndFrame2D();
    }

    protected abstract void BeginFrame2D();

    protected abstract void EndFrame2D();

    protected abstract void ExecuteCommand(
        RenderCommand command);
}