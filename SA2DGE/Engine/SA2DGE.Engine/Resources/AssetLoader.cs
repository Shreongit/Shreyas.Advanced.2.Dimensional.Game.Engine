namespace SA2DGE.Engine.Resources;

public interface IAssetLoader
{
    AssetType SupportedType { get; }

    bool CanLoad(string path);

    Asset Load(string path);
}

public abstract class AssetLoader<T> : IAssetLoader
    where T : Asset
{
    public abstract AssetType SupportedType { get; }

    public virtual bool CanLoad(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return File.Exists(path);
    }

    public Asset Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!CanLoad(path))
        {
            throw new FileNotFoundException(
                $"Unable to load asset from '{path}'.",
                path);
        }

        T asset = LoadAsset(path);

        asset.Load();

        return asset;
    }

    protected abstract T LoadAsset(string path);
}