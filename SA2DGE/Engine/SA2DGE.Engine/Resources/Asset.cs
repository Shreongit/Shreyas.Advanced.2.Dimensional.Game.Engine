namespace SA2DGE.Engine.Resources;

public abstract class Asset : IDisposable
{
    private bool _disposed;

    public Guid Id { get; }

    public string Name { get; }

    public string Path { get; }

    public AssetType Type { get; }

    public bool IsLoaded { get; private set; }

    protected Asset(
        string name,
        string path,
        AssetType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (type == AssetType.Unknown)
        {
            throw new ArgumentException(
                "Asset type cannot be Unknown.",
                nameof(type));
        }

        Id = Guid.NewGuid();
        Name = name;
        Path = path;
        Type = type;
    }

    public void Load()
    {
        ThrowIfDisposed();

        if (IsLoaded)
        {
            return;
        }

        OnLoad();

        IsLoaded = true;
    }

    public void Unload()
    {
        if (_disposed || !IsLoaded)
        {
            return;
        }

        OnUnload();

        IsLoaded = false;
    }

    protected virtual void OnLoad()
    {
    }

    protected virtual void OnUnload()
    {
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Unload();

        OnDispose();

        _disposed = true;
    }

    protected virtual void OnDispose()
    {
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    public override string ToString()
    {
        return $"{Type}: {Name}";
    }
}