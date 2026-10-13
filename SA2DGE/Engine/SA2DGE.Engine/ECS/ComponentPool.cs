namespace SA2DGE.Engine.ECS;

public sealed class ComponentPool<T> where T : Component
{
    private readonly Dictionary<EntityId, T> _components = new();

    public int Count => _components.Count;

    public void Add(EntityId entity, T component)
    {
        ArgumentNullException.ThrowIfNull(component);

        if (!entity.IsValid)
        {
            throw new ArgumentException(
                "Entity must be valid.",
                nameof(entity));
        }

        if (_components.ContainsKey(entity))
        {
            throw new InvalidOperationException(
                $"Entity {entity} already has component {typeof(T).Name}.");
        }

        _components.Add(entity, component);
    }

    public void Set(EntityId entity, T component)
    {
        ArgumentNullException.ThrowIfNull(component);

        if (!entity.IsValid)
        {
            throw new ArgumentException(
                "Entity must be valid.",
                nameof(entity));
        }

        _components[entity] = component;
    }

    public bool Remove(EntityId entity)
    {
        return _components.Remove(entity);
    }

    public bool Has(EntityId entity)
    {
        return _components.ContainsKey(entity);
    }

    public bool TryGet(
        EntityId entity,
        out T? component)
    {
        return _components.TryGetValue(
            entity,
            out component);
    }

    public T Get(EntityId entity)
    {
        if (!_components.TryGetValue(
                entity,
                out T? component))
        {
            throw new KeyNotFoundException(
                $"Entity {entity} does not have component {typeof(T).Name}.");
        }

        return component;
    }

    public void Clear()
    {
        _components.Clear();
    }

    public IEnumerable<KeyValuePair<EntityId, T>> Entries =>
        _components;
}