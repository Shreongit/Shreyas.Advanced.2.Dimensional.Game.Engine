namespace SA2DGE.Engine.Resources;

public readonly struct AssetHandle<T> : IEquatable<AssetHandle<T>>
    where T : Asset
{
    public Guid Id { get; }

    public bool IsValid => Id != Guid.Empty;

    public AssetHandle(Guid id)
    {
        Id = id;
    }

    public static AssetHandle<T> Invalid =>
        new(Guid.Empty);

    public bool Equals(AssetHandle<T> other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is AssetHandle<T> other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(
        AssetHandle<T> left,
        AssetHandle<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        AssetHandle<T> left,
        AssetHandle<T> right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return IsValid
            ? Id.ToString()
            : "Invalid Asset Handle";
    }
}