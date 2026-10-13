namespace SA2DGE.Engine.ECS;

public sealed class Entity
{
    private readonly World _world;

    public EntityId Id { get; }

    public bool IsValid =>
        _world.IsEntityAlive(Id);

    internal World World =>
        _world;

    internal Entity(
        World world,
        EntityId id)
    {
        ArgumentNullException.ThrowIfNull(world);

        _world = world;
        Id = id;
    }

    public bool Has<T>()
        where T : Component
    {
        EnsureValid();

        return _world.HasComponent<T>(Id);
    }

    public T Get<T>()
        where T : Component
    {
        EnsureValid();

        return _world.GetComponent<T>(Id);
    }

    public bool TryGet<T>(
        out T? component)
        where T : Component
    {
        if (!IsValid)
        {
            component = null;
            return false;
        }

        return _world.TryGetComponent(
            Id,
            out component);
    }

    public void Add<T>(
        T component)
        where T : Component
    {
        EnsureValid();
        ArgumentNullException.ThrowIfNull(component);

        _world.AddComponent(
            Id,
            component);
    }

    public void Set<T>(
        T component)
        where T : Component
    {
        EnsureValid();
        ArgumentNullException.ThrowIfNull(component);

        _world.SetComponent(
            Id,
            component);
    }

    public bool Remove<T>()
        where T : Component
    {
        EnsureValid();

        return _world.RemoveComponent<T>(Id);
    }

    public void Destroy()
    {
        if (IsValid)
        {
            _world.DestroyEntity(Id);
        }
    }

    private void EnsureValid()
    {
        if (!IsValid)
        {
            throw new InvalidOperationException(
                $"Entity {Id} is no longer valid.");
        }
    }

    public override string ToString() =>
        Id.ToString();
}