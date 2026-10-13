using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Animation;

public sealed class AnimationClip
{
    private readonly List<AnimationFrame> _frames = new();

    public string Name { get; }

    public float FrameRate { get; set; }

    public bool Loop { get; set; }

    public IReadOnlyList<AnimationFrame> Frames =>
        _frames;

    public int FrameCount =>
        _frames.Count;

    public float Duration =>
        FrameRate > 0.0f
            ? FrameCount / FrameRate
            : 0.0f;

    public AnimationClip(
        string name,
        float frameRate = 12.0f,
        bool loop = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (frameRate <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameRate));
        }

        Name = name;
        FrameRate = frameRate;
        Loop = loop;
    }

    public void AddFrame(
        TextureRegion region,
        float duration = 0.0f)
    {
        if (duration < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration));
        }

        _frames.Add(
            new AnimationFrame(
                region,
                duration));
    }

    public void AddFrame(
        Texture2D texture,
        Rectangle region,
        float duration = 0.0f)
    {
        ArgumentNullException.ThrowIfNull(texture);

        AddFrame(
            new TextureRegion(
                texture,
                region),
            duration);
    }

    public bool RemoveFrame(
        int index)
    {
        if ((uint)index >=
            (uint)_frames.Count)
        {
            return false;
        }

        _frames.RemoveAt(index);
        return true;
    }

    public void Clear()
    {
        _frames.Clear();
    }

    public AnimationFrame GetFrame(
        int index)
    {
        if ((uint)index >=
            (uint)_frames.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index));
        }

        return _frames[index];
    }

    public AnimationFrame GetFrameAtTime(
        float time)
    {
        if (_frames.Count == 0)
        {
            throw new InvalidOperationException(
                $"Animation clip '{Name}' contains no frames.");
        }

        if (time < 0.0f)
        {
            time = 0.0f;
        }

        float frameDuration =
            1.0f / FrameRate;

        int frameIndex =
            (int)(time / frameDuration);

        if (Loop)
        {
            frameIndex %= _frames.Count;
        }
        else
        {
            frameIndex =
                System.Math.Min(
                    frameIndex,
                    _frames.Count - 1);
        }

        return _frames[frameIndex];
    }

    public override string ToString()
    {
        return $"{Name} ({FrameCount} frames, {FrameRate} FPS)";
    }
}

public readonly struct AnimationFrame
{
    public TextureRegion Region { get; }

    public float Duration { get; }

    public AnimationFrame(
        TextureRegion region,
        float duration = 0.0f)
    {
        if (duration < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration));
        }

        Region = region;
        Duration = duration;
    }
}