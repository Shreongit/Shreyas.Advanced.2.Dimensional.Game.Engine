
namespace SA2DGE.Engine.Graphics;

/// <summary>
/// Advances through a SpriteSheet's frames over time.
/// </summary>
public sealed class SpriteAnimation
{
    private float _elapsedTime;

    public SpriteSheet SpriteSheet { get; }

    public float FrameDuration { get; }

    public int CurrentFrame { get; private set; }

    public bool IsPlaying { get; private set; }

    public bool Loop { get; set; }

    public SpriteAnimation(
        SpriteSheet spriteSheet,
        float frameDuration,
        bool loop = true)
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

        SpriteSheet = spriteSheet;
        FrameDuration = frameDuration;
        Loop = loop;
        CurrentFrame = 0;
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
    /// Stops playback and resets to the first frame.
    /// </summary>
    public void Stop()
    {
        IsPlaying = false;
        CurrentFrame = 0;
        _elapsedTime = 0.0f;
    }
    
    /// <summary>
    /// Restarts playback from the first frame.
    /// </summary>
    public void Restart()
    {
        CurrentFrame = 0;
        _elapsedTime = 0.0f;
        IsPlaying = true;
    }

    /// <summary>
    /// Selects a frame manually and resets its elapsed time.
    /// </summary>
    public void SetFrame(int frameIndex)
    {
        if (frameIndex < 0 ||
            frameIndex >= SpriteSheet.FrameCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameIndex),
                $"Frame index must be between 0 and {SpriteSheet.FrameCount - 1}.");
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

        if (!IsPlaying || SpriteSheet.FrameCount <= 1)
            return;

        _elapsedTime += deltaTime;

        while (_elapsedTime >= FrameDuration)
        {
            _elapsedTime -= FrameDuration;

            if (CurrentFrame < SpriteSheet.FrameCount - 1)
            {
                CurrentFrame++;
            }
            else if (Loop)
            {
                CurrentFrame = 0;
            }
            else
            {
                CurrentFrame = SpriteSheet.FrameCount - 1;
                _elapsedTime = 0.0f;
                IsPlaying = false;
                break;
            }
        }
    }
}
