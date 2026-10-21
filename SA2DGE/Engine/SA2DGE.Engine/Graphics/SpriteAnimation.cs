
namespace SA2DGE.Engine.Graphics;

/// <summary>
/// Advances through a selected range of a SpriteSheet's frames over time.
/// </summary>
public sealed class SpriteAnimation
{
    private float _elapsedTime;

    public SpriteSheet SpriteSheet { get; }

    public float FrameDuration { get; }

    /// <summary>
    /// First frame included in this animation.
    /// </summary>
    public int StartFrame { get; }

    /// <summary>
    /// Number of frames included in this animation.
    /// </summary>
    public int FrameCount { get; }

    /// <summary>
    /// Index of the currently displayed frame in the full sprite sheet.
    /// </summary>
    public int CurrentFrame { get; private set; }

    public bool IsPlaying { get; private set; }

    public bool Loop { get; set; }

    public int EndFrame => StartFrame + FrameCount - 1;

    public SpriteAnimation(
        SpriteSheet spriteSheet,
        float frameDuration,
        bool loop = true,
        int startFrame = 0,
        int? frameCount = null)
    {
        ArgumentNullException.ThrowIfNull(spriteSheet);

        if (frameDuration <= 0.0f ||
            float.IsNaN(frameDuration) ||
            float.IsInfinity(frameDuration))
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameDuration),
                "Frame duration must be a finite value greater than zero.");
        }

        if (startFrame < 0 || startFrame >= spriteSheet.FrameCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startFrame),
                "Start frame must be within the sprite sheet.");
        }

        int resolvedFrameCount = frameCount
            ?? (spriteSheet.FrameCount - startFrame);

        if (resolvedFrameCount <= 0 ||
            resolvedFrameCount > spriteSheet.FrameCount - startFrame)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameCount),
                "Frame count must fit within the sprite sheet from startFrame.");
        }

        SpriteSheet = spriteSheet;
        FrameDuration = frameDuration;
        Loop = loop;
        StartFrame = startFrame;
        FrameCount = resolvedFrameCount;
        CurrentFrame = StartFrame;
        IsPlaying = false;
    }

    /// <summary>
    /// Starts playback from the current frame.
    /// </summary>
    public void Play()
    {
        IsPlaying = true;
    }

    /// <summary>
    /// Stops playback and preserves the current frame.
    /// </summary>
    public void Pause()
    {
        IsPlaying = false;
    }

    /// <summary>
    /// Stops playback and resets to the first frame of this animation.
    /// </summary>
    public void Stop()
    {
        IsPlaying = false;
        CurrentFrame = StartFrame;
        _elapsedTime = 0.0f;
    }

    /// <summary>
    /// Restarts playback from the first frame of this animation.
    /// </summary>
    public void Restart()
    {
        CurrentFrame = StartFrame;
        _elapsedTime = 0.0f;
        IsPlaying = true;
    }

    /// <summary>
    /// Selects a frame by its absolute index in the sprite sheet.
    /// </summary>
    public void SetFrame(int frameIndex)
    {
        if (frameIndex < StartFrame || frameIndex > EndFrame)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameIndex),
                $"Frame index must be between {StartFrame} and {EndFrame}.");
        }

        CurrentFrame = frameIndex;
        _elapsedTime = 0.0f;
    }

    /// <summary>
    /// Advances playback using elapsed time in seconds.
    /// </summary>
    public void Update(float deltaTime)
    {
        if (deltaTime < 0.0f ||
            float.IsNaN(deltaTime) ||
            float.IsInfinity(deltaTime))
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTime),
                "Delta time must be a finite, non-negative value.");
        }

        if (!IsPlaying || FrameCount <= 1)
            return;

        _elapsedTime += deltaTime;

        while (_elapsedTime >= FrameDuration)
        {
            _elapsedTime -= FrameDuration;

            if (CurrentFrame < EndFrame)
            {
                CurrentFrame++;
            }
            else if (Loop)
            {
                CurrentFrame = StartFrame;
            }
            else
            {
                CurrentFrame = EndFrame;
                _elapsedTime = 0.0f;
                IsPlaying = false;
                break;
            }
        }
    }
}
