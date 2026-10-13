using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Audio;

public sealed class AudioSource
{
    private float _playbackTime;

    public AudioClip? Clip { get; private set; }

    public Vector2 Position { get; set; }

    public float Volume { get; set; }

    public float Pitch { get; set; }

    public bool Loop { get; set; }

    public bool IsPlaying { get; private set; }

    public bool IsPaused { get; private set; }

    public float PlaybackTime =>
        _playbackTime;

    public float Duration =>
        Clip?.Duration ?? 0.0f;

    public float RemainingTime =>
        MathF.Max(
            0.0f,
            Duration - _playbackTime);

    public AudioSource(
        AudioClip? clip = null)
    {
        Clip = clip;
        Position = Vector2.Zero;
        Volume = 1.0f;
        Pitch = 1.0f;
        Loop = false;
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
            _playbackTime = 0.0f;
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
        _playbackTime = 0.0f;
    }

    public void Rewind()
    {
        _playbackTime = 0.0f;
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

        float effectivePitch =
            MathF.Max(
                Pitch,
                0.0f);

        _playbackTime +=
            deltaTime * effectivePitch;

        if (Duration <= 0.0f)
        {
            Stop();
            return;
        }

        if (Loop)
        {
            _playbackTime %= Duration;
            return;
        }

        if (_playbackTime >= Duration)
        {
            _playbackTime = Duration;
            IsPlaying = false;
            IsPaused = false;
        }
    }

    public float GetDistance(
        AudioListener listener)
    {
        ArgumentNullException.ThrowIfNull(listener);

        return Vector2.Distance(
            Position,
            listener.Position);
    }

    public float GetAttenuatedVolume(
        AudioListener listener,
        float referenceDistance = 1.0f,
        float maxDistance = 100.0f)
    {
        ArgumentNullException.ThrowIfNull(listener);

        if (referenceDistance <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(referenceDistance));
        }

        if (maxDistance < referenceDistance)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxDistance));
        }

        float distance =
            GetDistance(listener);

        if (distance <= referenceDistance)
        {
            return Volume *
                   listener.Volume;
        }

        if (distance >= maxDistance)
        {
            return 0.0f;
        }

        float attenuation =
            1.0f -
            (distance - referenceDistance) /
            (maxDistance - referenceDistance);

        return Volume *
               listener.Volume *
               MathUtils.Clamp(
                   attenuation,
                   0.0f,
                   1.0f);
    }
}