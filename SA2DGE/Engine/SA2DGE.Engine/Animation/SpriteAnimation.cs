using SA2DGE.Engine.Graphics.Sprites;
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Animation;

public sealed class SpriteAnimation
{
    public Sprite Sprite { get; }

    public AnimationController Controller { get; }

    public bool IsPlaying =>
        Controller.IsPlaying;

    public int CurrentFrame =>
        Controller.CurrentFrame;

    public string? CurrentState =>
        Controller.CurrentStateName;

    public SpriteAnimation(
        Sprite sprite,
        AnimationController controller)
    {
        ArgumentNullException.ThrowIfNull(sprite);
        ArgumentNullException.ThrowIfNull(controller);

        Sprite = sprite;
        Controller = controller;
    }

    public void Update(
        float deltaTime)
    {
        Controller.Update(deltaTime);

        AnimationState? state =
            Controller.CurrentState;

        if (state is null ||
            state.Clip.FrameCount == 0)
        {
            return;
        }

        AnimationFrame frame =
            state.GetCurrentFrame();

        Sprite.SetRegion(frame.Region);
    }

    public bool Play(
        string stateName,
        bool restart = false)
    {
        return Controller.Play(
            stateName,
            restart);
    }

    public void Pause()
    {
        Controller.Pause();
    }

    public void Stop()
    {
        Controller.Stop();
    }

    public void Reset()
    {
        Controller.Reset();

        AnimationState? state =
            Controller.CurrentState;

        if (state is null ||
            state.Clip.FrameCount == 0)
        {
            return;
        }

        Sprite.SetRegion(
            state.GetCurrentFrame().Region);
    }

    public void SetSpriteSize(
        Vector2 size)
    {
        Sprite.Size = size;
    }

    public void SetColor(
        Color color)
    {
        Sprite.Color = color;
    }

    public void SetLayer(
        int layer)
    {
        Sprite.Layer = layer;
    }

    public void SetFlip(
        bool flipX,
        bool flipY)
    {
        Sprite.FlipX = flipX;
        Sprite.FlipY = flipY;
    }

    public TextureRegion GetCurrentRegion()
    {
        AnimationState? state =
            Controller.CurrentState;

        if (state is null)
        {
            throw new InvalidOperationException(
                "The animation controller has no current state.");
        }

        return state.GetCurrentFrame().Region;
    }
}