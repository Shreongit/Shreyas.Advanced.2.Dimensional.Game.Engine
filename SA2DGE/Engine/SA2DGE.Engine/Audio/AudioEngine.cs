namespace SA2DGE.Engine.Audio;

public class AudioEngine : IDisposable
{
    private readonly List<AudioSource> _sources = new();

    private bool _initialized;
    private bool _disposed;

    public AudioListener Listener { get; }

    public Music? CurrentMusic { get; private set; }

    public float MasterVolume { get; private set; }

    public bool IsInitialized =>
        _initialized;

    public int SourceCount =>
        _sources.Count;

    public IReadOnlyList<AudioSource> Sources =>
        _sources;

    public AudioEngine()
    {
        Listener = new AudioListener();
        MasterVolume = 1.0f;
    }

    public void Initialize()
    {
        ThrowIfDisposed();

        if (_initialized)
        {
            return;
        }

        OnInitialize();
        _initialized = true;
    }

    public void AddSource(
        AudioSource source)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(source);

        if (_sources.Contains(source))
        {
            return;
        }

        _sources.Add(source);
    }

    public bool RemoveSource(
        AudioSource source)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(source);

        return _sources.Remove(source);
    }

    public void Play(
        AudioSource source)
    {
        ThrowIfDisposed();
        EnsureInitialized();
        ArgumentNullException.ThrowIfNull(source);

        if (!_sources.Contains(source))
        {
            AddSource(source);
        }

        source.Play();
        OnPlay(source);
    }

    public void Pause(
        AudioSource source)
    {
        ThrowIfDisposed();
        EnsureInitialized();
        ArgumentNullException.ThrowIfNull(source);

        source.Pause();
        OnPause(source);
    }

    public void Resume(
        AudioSource source)
    {
        ThrowIfDisposed();
        EnsureInitialized();
        ArgumentNullException.ThrowIfNull(source);

        source.Resume();
        OnResume(source);
    }

    public void Stop(
        AudioSource source)
    {
        ThrowIfDisposed();
        EnsureInitialized();
        ArgumentNullException.ThrowIfNull(source);

        source.Stop();
        OnStop(source);
    }

    public void PlayMusic(
        Music music)
    {
        ThrowIfDisposed();
        EnsureInitialized();
        ArgumentNullException.ThrowIfNull(music);

        if (ReferenceEquals(
                CurrentMusic,
                music))
        {
            music.Play();
            return;
        }

        CurrentMusic?.Stop();

        CurrentMusic = music;
        CurrentMusic.Play();

        OnPlayMusic(music);
    }

    public void PauseMusic()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        CurrentMusic?.Pause();
    }

    public void ResumeMusic()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        CurrentMusic?.Resume();
    }

    public void StopMusic()
    {
        ThrowIfDisposed();
        EnsureInitialized();

        CurrentMusic?.Stop();
    }

    public void SetMasterVolume(
        float volume)
    {
        ThrowIfDisposed();

        MasterVolume =
            System.Math.Clamp(
                volume,
                0.0f,
                1.0f);

        OnMasterVolumeChanged(
            MasterVolume);
    }

    public void Update(
        float deltaTime)
    {
        ThrowIfDisposed();
        EnsureInitialized();

        if (deltaTime < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTime));
        }

        foreach (AudioSource source in _sources)
        {
            source.Update(deltaTime);

            OnUpdateSource(
                source,
                source.GetAttenuatedVolume(
                    Listener));
        }

        CurrentMusic?.Update(deltaTime);

        if (CurrentMusic is not null)
        {
            OnUpdateMusic(
                CurrentMusic,
                CurrentMusic.Volume *
                MasterVolume);
        }
    }

    public void StopAll()
    {
        ThrowIfDisposed();

        foreach (AudioSource source in _sources)
        {
            source.Stop();
        }

        CurrentMusic?.Stop();
    }

    public void Shutdown()
    {
        if (!_initialized)
        {
            return;
        }

        StopAll();
        OnShutdown();

        _initialized = false;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Shutdown();

        _sources.Clear();
        CurrentMusic = null;

        _disposed = true;
    }

    protected virtual void OnInitialize()
    {
    }

    protected virtual void OnPlay(
        AudioSource source)
    {
    }

    protected virtual void OnPause(
        AudioSource source)
    {
    }

    protected virtual void OnResume(
        AudioSource source)
    {
    }

    protected virtual void OnStop(
        AudioSource source)
    {
    }

    protected virtual void OnPlayMusic(
        Music music)
    {
    }

    protected virtual void OnUpdateSource(
        AudioSource source,
        float effectiveVolume)
    {
    }

    protected virtual void OnUpdateMusic(
        Music music,
        float effectiveVolume)
    {
    }

    protected virtual void OnMasterVolumeChanged(
        float volume)
    {
    }

    protected virtual void OnShutdown()
    {
    }

    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException(
                "Audio engine has not been initialized.");
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}