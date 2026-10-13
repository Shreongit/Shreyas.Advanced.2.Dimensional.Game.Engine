namespace SA2DGE.Engine.Audio;

public sealed class Music
{
    public AudioClip? Clip { get; private set; }

    public float Volume { get; set; }

    public bool Loop { get; set; }

    public bool IsPlaying { get; private set; }

    public bool IsPaused { get; private set; }

    public float PlaybackTime { get; private set; }

    public float Duration =>
        Clip?.Duration ?? 0.0f;

    public Music(
        AudioClip? clip = null)
    {
        Clip = clip;
        Volume = 1.0f;
        Loop = true;
    }

    public void SetClip(
        AudioClip? clip)
    {
        Stop();

        Clip = clip;
    }

    public void Play()
    {
        if (Clip is null)
        {
            throw new InvalidOperationException(
                "An audio clip must be assigned before playback.");
        }

        if (Clip.IsDisposed)
        {
            throw new ObjectDisposedException(
                nameof(Clip));
        }

        if (!IsPaused)
        {
            PlaybackTime = 0.0f;
        }

        IsPlaying = true;
        IsPaused = false;
    }

    public void Pause()
    {
        if (!IsPlaying)
        {
            return;
        }

        IsPlaying = false;
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsPaused ||
            Clip is null)
        {
            return;
        }

        IsPlaying = true;
        IsPaused = false;
    }

    public void Stop()
    {
        IsPlaying = false;
        IsPaused = false;
        PlaybackTime = 0.0f;
    }

    public void Rewind()
    {
        PlaybackTime = 0.0f;
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
            Clip is null ||
            deltaTime == 0.0f)
        {
            return;
        }

        PlaybackTime += deltaTime;

        if (Duration <= 0.0f)
        {
            Stop();
            return;
        }

        if (Loop)
        {
            PlaybackTime %= Duration;
            return;
        }

        if (PlaybackTime >= Duration)
        {
            PlaybackTime = Duration;
            IsPlaying = false;
            IsPaused = false;
        }
    }
}