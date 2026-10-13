namespace SA2DGE.Engine.Animation;

public sealed class AnimationState
{
    public string Name { get; }

    public AnimationClip Clip { get; }

    public float Time { get; private set; }

    public bool IsPlaying { get; private set; }

    public bool IsFinished =>
        !Clip.Loop &&
        Time >= Clip.Duration;

    public int CurrentFrame
    {
        get
        {
            if (Clip.FrameCount == 0)
            {
                return -1;
            }

            float frameDuration =
                1.0f / Clip.FrameRate;

            int frame =
                (int)(Time / frameDuration);

            return Clip.Loop
                ? frame % Clip.FrameCount
                : System.Math.Min(
                    frame,
                    Clip.FrameCount - 1);
        }
    }

    public AnimationState(
        string name,
        AnimationClip clip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(clip);

        Name = name;
        Clip = clip;
    }

    public void Play()
    {
        IsPlaying = true;
    }

    public void Pause()
    {
        IsPlaying = false;
    }

    public void Stop()
    {
        Time = 0.0f;
        IsPlaying = false;
    }

    public void Reset()
    {
        Time = 0.0f;
    }

    public void Update(
        float deltaTime)
    {
        if (deltaTime < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTime));
        }

        if (!IsPlaying ||
            Clip.FrameCount == 0 ||
            deltaTime == 0.0f)
        {
            return;
        }

        Time += deltaTime;

        if (Clip.Loop)
        {
            float duration =
                Clip.Duration;

            if (duration > 0.0f)
            {
                Time %= duration;
            }
        }
        else if (Time >= Clip.Duration)
        {
            Time = Clip.Duration;
            IsPlaying = false;
        }
    }

    public AnimationFrame GetCurrentFrame()
    {
        if (Clip.FrameCount == 0)
        {
            throw new InvalidOperationException(
                $"Animation clip '{Clip.Name}' contains no frames.");
        }

        return Clip.GetFrameAtTime(Time);
    }

    public override string ToString()
    {
        return $"{Name}: Frame {CurrentFrame}, Time {Time:0.###}";
    }
}