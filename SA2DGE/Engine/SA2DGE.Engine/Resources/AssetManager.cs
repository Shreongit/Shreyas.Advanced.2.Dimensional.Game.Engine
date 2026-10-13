namespace SA2DGE.Engine.Resources;

public sealed class AssetManager : IDisposable
{
    private readonly AssetCache _cache = new();
    private readonly Dictionary<AssetType, IAssetLoader> _loaders = new();

    private bool _disposed;

    public int AssetCount => _cache.Count;

    public void RegisterLoader(IAssetLoader loader)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(loader);

        _loaders[loader.SupportedType] = loader;
    }

    public bool UnregisterLoader(AssetType type)
    {
        ThrowIfDisposed();

        return _loaders.Remove(type);
    }

    public AssetHandle<T> Load<T>(
        string path,
        AssetType type)
        where T : Asset
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!_loaders.TryGetValue(
                type,
                out IAssetLoader? loader))
        {
            throw new InvalidOperationException(
                $"No asset loader is registered for type '{type}'.");
        }

        if (!loader.CanLoad(path))
        {
            throw new InvalidOperationException(
                $"Loader '{loader.GetType().Name}' cannot load '{path}'.");
        }

        Asset asset = loader.Load(path);

        if (asset is not T typedAsset)
        {
            asset.Dispose();

            throw new InvalidOperationException(
                $"Asset loader returned '{asset.GetType().Name}', " +
                $"but '{typeof(T).Name}' was expected.");
        }

        _cache.Add(typedAsset);

        return new AssetHandle<T>(typedAsset.Id);
    }

    public void Add(Asset asset)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(asset);

        _cache.Add(asset);
    }

    public T Get<T>(
        AssetHandle<T> handle)
        where T : Asset
    {
        ThrowIfDisposed();

        if (!_cache.TryGet(
                handle,
                out T? asset))
        {
            throw new KeyNotFoundException(
                $"Asset '{handle.Id}' was not found.");
        }

        return asset!;
    }

    public bool TryGet<T>(
        AssetHandle<T> handle,
        out T? asset)
        where T : Asset
    {
        if (_disposed)
        {
            asset = null;
            return false;
        }

        return _cache.TryGet(
            handle,
            out asset);
    }

    public bool TryGetByPath(
        string path,
        out Asset? asset)
    {
        if (_disposed)
        {
            asset = null;
            return false;
        }

        return _cache.TryGetByPath(
            path,
            out asset);
    }

    public bool Unload<T>(
        AssetHandle<T> handle)
        where T : Asset
    {
        ThrowIfDisposed();

        if (!handle.IsValid)
        {
            return false;
        }

        return _cache.Remove(handle.Id);
    }

    public bool Unload(Guid id)
    {
        ThrowIfDisposed();

        return _cache.Remove(id);
    }

    public void UnloadAll()
    {
        ThrowIfDisposed();

        _cache.Clear();
    }

    public bool HasLoader(AssetType type)
    {
        return !_disposed &&
               _loaders.ContainsKey(type);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _cache.Dispose();
        _loaders.Clear();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}