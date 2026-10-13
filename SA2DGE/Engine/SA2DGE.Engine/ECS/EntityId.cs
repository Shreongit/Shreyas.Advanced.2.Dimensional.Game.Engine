namespace SA2DGE.Engine.ECS;

public readonly struct EntityId : IEquatable<EntityId>
{
    public static readonly EntityId Invalid = new(0);

    public uint Value { get; }

    public bool IsValid => Value != 0;

    public EntityId(uint value)
    {
        Value = value;
    }

    public bool Equals(EntityId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is EntityId other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(
        EntityId left,
        EntityId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(
        EntityId left,
        EntityId right)
    {
        return left.Value != right.Value;
    }

    public override string ToString()
    {
        return IsValid
            ? $"Entity({Value})"
            : "Entity(Invalid)";
    }
}