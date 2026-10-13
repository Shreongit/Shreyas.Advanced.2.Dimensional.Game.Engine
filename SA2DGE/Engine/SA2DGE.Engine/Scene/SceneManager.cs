namespace SA2DGE.Engine.Scene;

public sealed class SceneManager : IDisposable
{
    private readonly Dictionary<string, Scene> _scenes =
        new(StringComparer.OrdinalIgnoreCase);

    private Scene? _activeScene;
    private bool _disposed;

    public Scene? ActiveScene => _activeScene;

    public int SceneCount => _scenes.Count;

    public IReadOnlyCollection<Scene> Scenes =>
        _scenes.Values;

    public void Add(Scene scene)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(scene);

        if (_scenes.ContainsKey(scene.Name))
        {
            throw new InvalidOperationException(
                $"A scene named '{scene.Name}' is already registered.");
        }

        _scenes.Add(scene.Name, scene);
    }

    public bool Remove(string name)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_scenes.TryGetValue(name, out Scene? scene))
        {
            return false;
        }

        if (ReferenceEquals(scene, _activeScene))
        {
            Unload();
        }

        _scenes.Remove(name);
        scene.Dispose();

        return true;
    }

    public Scene Get(string name)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_scenes.TryGetValue(
                name,
                out Scene? scene))
        {
            throw new KeyNotFoundException(
                $"Scene '{name}' is not registered.");
        }

        return scene;
    }

    public bool TryGet(
        string name,
        out Scene? scene)
    {
        if (_disposed)
        {
            scene = null;
            return false;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _scenes.TryGetValue(
            name,
            out scene);
    }

    public void Load(string name)
    {
        ThrowIfDisposed();

        Scene scene = Get(name);

        if (ReferenceEquals(scene, _activeScene))
        {
            return;
        }

        _activeScene?.Shutdown();

        _activeScene = scene;

        _activeScene.Initialize();
    }

    public void Load(Scene scene)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(scene);

        if (!_scenes.ContainsKey(scene.Name))
        {
            Add(scene);
        }

        Load(scene.Name);
    }

    public void Unload()
    {
        ThrowIfDisposed();

        if (_activeScene is null)
        {
            return;
        }

        _activeScene.Shutdown();
        _activeScene = null;
    }

    public void Update(float deltaTime)
    {
        ThrowIfDisposed();

        _activeScene?.Update(deltaTime);
    }

    public void Clear()
    {
        ThrowIfDisposed();

        Unload();

        foreach (Scene scene in _scenes.Values)
        {
            scene.Dispose();
        }

        _scenes.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Clear();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}