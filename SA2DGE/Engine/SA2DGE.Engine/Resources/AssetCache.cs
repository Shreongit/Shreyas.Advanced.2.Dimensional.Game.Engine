namespace SA2DGE.Engine.Resources;

public sealed class AssetCache : IDisposable
{
    private readonly Dictionary<Guid, Asset> _assets = new();
    private readonly Dictionary<string, Guid> _paths =
        new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    public int Count => _assets.Count;

    public void Add(Asset asset)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(asset);

        if (_assets.ContainsKey(asset.Id))
        {
            throw new InvalidOperationException(
                $"Asset '{asset.Name}' is already cached.");
        }

        if (_paths.ContainsKey(asset.Path))
        {
            throw new InvalidOperationException(
                $"An asset with path '{asset.Path}' is already cached.");
        }

        _assets.Add(asset.Id, asset);
        _paths.Add(asset.Path, asset.Id);
    }

    public bool Remove(Guid id)
    {
        ThrowIfDisposed();

        if (!_assets.Remove(id, out Asset? asset))
        {
            return false;
        }

        _paths.Remove(asset.Path);
        asset.Dispose();

        return true;
    }

    public bool Contains(Guid id)
    {
        return !_disposed &&
               _assets.ContainsKey(id);
    }

    public bool ContainsPath(string path)
    {
        if (_disposed)
        {
            return false;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return _paths.ContainsKey(path);
    }

    public bool TryGet(
        Guid id,
        out Asset? asset)
    {
        if (_disposed)
        {
            asset = null;
            return false;
        }

        return _assets.TryGetValue(
            id,
            out asset);
    }

    public bool TryGet<T>(
        AssetHandle<T> handle,
        out T? asset)
        where T : Asset
    {
        asset = null;

        if (!handle.IsValid ||
            _disposed ||
            !_assets.TryGetValue(
                handle.Id,
                out Asset? value))
        {
            return false;
        }

        if (value is not T typedAsset)
        {
            return false;
        }

        asset = typedAsset;
        return true;
    }

    public bool TryGetByPath(
        string path,
        out Asset? asset)
    {
        asset = null;

        if (_disposed)
        {
            return false;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!_paths.TryGetValue(
                path,
                out Guid id))
        {
            return false;
        }

        return _assets.TryGetValue(
            id,
            out asset);
    }

    public IReadOnlyCollection<Asset> Assets =>
        _assets.Values;

    public void Clear()
    {
        ThrowIfDisposed();

        foreach (Asset asset in _assets.Values)
        {
            asset.Dispose();
        }

        _assets.Clear();
        _paths.Clear();
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